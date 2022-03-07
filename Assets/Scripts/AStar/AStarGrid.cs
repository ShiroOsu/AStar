using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public static class Extensions
    {
        public static Vector3Int Vector3ToVector3Int(Vector3 v)
        {
            return new Vector3Int((int) v.x, (int) v.y, (int) v.z);
        }
    }

    public enum CellState
    {
        Unchecked,
        Closed,
        Open,
    }

    public class Cell : IHeapItem<Cell>
    {
        public Cell(Vector3 worldPos, int gridX, int gridY, CellState cellState = CellState.Unchecked)
        {
            WorldPos = worldPos;
            CellState = cellState;
            x = gridX;
            y = gridY;
        }
        
        public Vector3 WorldPos;
        public CellState CellState;
        public Cell Parent;
        public float CellCost;
        public float G { get; set; }
        public float H { get; set; }
        private float F => G + H;
        public readonly int x, y;
        
        public int CompareTo(Cell other)
        {
            var compare = F.CompareTo(other.F);
            if (F == 0)
            {
                compare = H.CompareTo(other.H);
            }

            return -compare;
        }

        public int HeapIndex { get; set; }
    }

    public class AStarGrid
    {
        private readonly LayerMask m_ImpassableMask;
        public Vector2Int GridSize;
        public readonly Cell[,] Grid;

        private readonly List<Cell> m_AdjacentCellsList;
        private readonly Collider[] m_Hits = new Collider[1];
        private readonly Vector3 m_HalfExtents;

        public AStarGrid(Terrain terrain, float cellRadius, LayerMask impassable)
        {
            var terrainData = terrain.terrainData.size;
            m_ImpassableMask = impassable;
            var size = new Vector2Int((int) terrainData.x, (int) terrainData.z);

            Grid = new Cell[size.x, size.y];
            GridSize = size;
            m_AdjacentCellsList = new();

            m_HalfExtents = Vector3.one * cellRadius;
            SetCellPosition(terrain, cellRadius, ref size);
        }

        private void SetCellPosition(Terrain terrain, float cellRadius, ref Vector2Int gridSize)
        {
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var worldPos = new Vector3(cellRadius * 2f * x + cellRadius, cellRadius,
                        cellRadius * 2f * y + cellRadius);

                    var terrainHeight = terrain.SampleHeight(worldPos);
                    if (terrainHeight > 0.1f)
                    {
                        worldPos.y = terrainHeight + cellRadius; // (cellRadius * 2f)
                    }

                    Grid[x, y] = new Cell(worldPos, x, y);

                    // Is cell on Terrain Border 
                    if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
                    {
                        Grid[x, y].CellState = CellState.Closed;
                        continue;
                    }

                    // Is cell on impassable terrain
                    var overlaps = Physics.OverlapBoxNonAlloc(worldPos, m_HalfExtents, m_Hits, Quaternion.identity,
                        m_ImpassableMask);
                    if (overlaps > 0)
                    {
                        Grid[x, y].CellState = CellState.Closed;
                    }
                }
            }
        }

        public List<Cell> GetAdjacentCells(Cell currentCell)
        {
            m_AdjacentCellsList.Clear();
            
            // Max number of adjacent cells is 8
            for (int i = 0; i < 8; i++)
            {
                ref var offset = ref GridDirection.AllDirections[i].Vector;
                var offsetX = currentCell.x + offset.x;
                var offsetY = currentCell.y + offset.y;

                // Out of bounds
                if (offsetX < 0 || offsetX > GridSize.x - 1 || offsetY < 0 || offsetY > GridSize.y - 1)
                {
                    continue;
                }

                m_AdjacentCellsList.Add(Grid[offsetX, offsetY]);
            }

            return m_AdjacentCellsList;
        }
    }

    public class GridDirection
    {
        public Vector2Int Vector;

        private GridDirection(int x, int y)
        {
            Vector = new Vector2Int(x, y);
        }

        private static readonly GridDirection None = new(0, 0);
        private static readonly GridDirection North = new(0, 1);
        private static readonly GridDirection South = new(0, -1);
        private static readonly GridDirection East = new(1, 0);
        private static readonly GridDirection West = new(-1, 0);
        private static readonly GridDirection NorthEast = new(1, 1);
        private static readonly GridDirection NorthWest = new(-1, 1);
        private static readonly GridDirection SouthEast = new(1, -1);
        private static readonly GridDirection SouthWest = new(-1, -1);

        public static readonly GridDirection[] AllDirections =
        {
            North,
            NorthEast,
            NorthWest,
            South,
            SouthEast,
            SouthWest,
            East,
            West
        };
    }
}
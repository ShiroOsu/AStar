using System;
using System.Collections.Generic;
using Player;
using Singleton;
using UnityEngine;

namespace AStar
{
    public class AStarController : Singleton<AStarController>
    {
        public PlayerScript playerScript;
        public float cellRadius;
        public Terrain terrain;
        public LayerMask impassableMask;
        
        private AStarGrid m_AStarGrid;
        private Cell[,] m_Grid;
        private Vector3Int m_StartLocation;
        private Vector3 m_EndLocation;

        public List<Cell> Path;
        private Heap<Cell> m_OpenCells;
        private List<Cell> m_ClosedCells;
        private int m_Size;

        private void Awake()
        {
            m_AStarGrid = new AStarGrid(terrain, cellRadius, impassableMask);
            
            m_Grid = m_AStarGrid.Grid;
            m_Size = m_AStarGrid.GridSize.x * m_AStarGrid.GridSize.y;
            m_ClosedCells = new List<Cell>();
            
            playerScript.CalculateAStarPath += CalculatePath;
        }

        private void CalculatePath(in Vector3 targetPosition, in Vector3Int currentPosition)
        {
            var startCell = m_Grid[currentPosition.x, currentPosition.z];
            var endCell = m_Grid[(int) targetPosition.x, (int) targetPosition.z];

            if (endCell.CellState == CellState.Closed || endCell == startCell)
            {
                return;
            }
            
            m_OpenCells = new Heap<Cell>(m_Size);

            AStar(startCell, endCell);
        }

        private void AStar(Cell startCell, Cell endCell)
        {
            m_OpenCells.Add(startCell);

            while (m_OpenCells.Count > 0)
            {
                var cell = m_OpenCells.RemoveFirstItem();
                cell.CellState = CellState.Closed;
                m_ClosedCells.Add(cell);

                if (cell == endCell)
                {
                    RetraceAStarPath(startCell, endCell);
                    return;
                }
                
                var adjacentCells = m_AStarGrid.GetAdjacentCells(cell);

                for (int i = 0; i < adjacentCells.Count; i++)
                {
                    var adjacentCell = adjacentCells[i];

                    if (adjacentCell.CellState == CellState.Closed)
                    {
                        continue;
                    }

                    if (adjacentCell.CellState == CellState.Unchecked)
                    {
                        adjacentCell.CellState = CellState.Open;
                    }

                    var distToAdjacentCell = cell.G + GetCellDistance(cell, adjacentCell);

                    if (!(distToAdjacentCell < adjacentCell.G) && m_OpenCells.Contains(adjacentCell))
                        continue;
                    
                    adjacentCell.G = distToAdjacentCell;
                    adjacentCell.H = GetCellDistance(adjacentCell, endCell);
                    adjacentCell.Parent = cell;

                    if (!m_OpenCells.Contains(adjacentCell))
                    {
                        m_OpenCells.Add(adjacentCell);
                    } else m_OpenCells.UpdateItem(adjacentCell);
                }
            }
        }

        private void RetraceAStarPath(Cell startCell, Cell endCell)
        {
            Path = new List<Cell>();
            
            var currentCell = endCell;

            while (currentCell != startCell)
            {
                Path.Add(currentCell);
                currentCell = currentCell.Parent;
            }
            
            Path.Reverse();
            playerScript.AStarPath = Path;
            ResetCells();
        }

        private static float GetCellDistance(Cell c1, Cell c2)
        {
            return Vector3.Distance(c1.WorldPos, c2.WorldPos);
        }

        private void ResetCells()
        {
            foreach (var cell in m_ClosedCells)
            {
                cell.CellState = CellState.Unchecked;
            }
            m_ClosedCells.Clear();
        }

        public bool AStarGridInitialized()
        {
            return m_AStarGrid != null;
        }
        
        public Vector2Int GetGridSize()
        {
            return m_AStarGrid.GridSize;
        }

        public Cell[,] GetGrid()
        {
            return m_Grid;
        }
    }
}
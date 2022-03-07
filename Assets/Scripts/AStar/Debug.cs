using UnityEngine;

namespace AStar
{
    public class Debug : MonoBehaviour
    {
        public float cellRadius;
        public AStarController aStarController;        
        private Vector2Int m_GridSize;
        private Cell[,] m_Grid;

        private void Init()
        {
            m_GridSize = aStarController.GetGridSize();
            m_Grid = aStarController.GetGrid();
        }
        
        private void OnDrawGizmos()
        {
            if (!aStarController.AStarGridInitialized())
            {
                return;
            }
            
            Init();

            for (int i = 0; i < m_GridSize.x; i++)
            {
                for (int j = 0; j < m_GridSize.y; j++)
                {
                    var center = new Vector3(cellRadius * 2f * i + cellRadius, cellRadius, cellRadius * 2f * j + cellRadius);
                    var size = Vector3.one * cellRadius * 2f;
                    var cell = m_Grid[i, j];

                    switch (cell.CellState)
                    {
                          case CellState.Unchecked:
                              continue;
                          
                          case CellState.Open:
                              Gizmos.color = Color.green;
                              break;
                          
                          case CellState.Closed:
                              Gizmos.color = Color.red;
                              break;
                          
                          default:
                              print("default");
                              break;
                    }

                    if (aStarController.Path != null)
                    {
                        if (aStarController.Path.Contains(cell))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }
}

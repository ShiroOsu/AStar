using System.Collections.Generic;
using AStar;
using Interfaces;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Player
{
    public class PlayerScript : MonoBehaviour
    {
        public UnityEngine.Camera mainCamera;
        public LayerMask unitMask, groundMask;
        private GameObject m_Unit;

        public event Action<Vector3, Vector3Int> CalculateAStarPath;
        public delegate void Action<T, T1>(in T vector3, in T1 vector3Int);
        
        public List<Cell> AStarPath { get; set; }

        private void Update()
        {
            // left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, unitMask))
                {
                    if (hit.transform.TryGetComponent(out IUnit _))
                    {
                        m_Unit = hit.transform.gameObject;
                    }
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                // Right mouse button
                if (m_Unit)
                {
                    var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask))
                    {
                        m_Unit.TryGetComponent(out IUnit u);

                        var v3 = hit.point;
                        var v3I = u.GetPosition();
                        CalculateAStarPath?.Invoke(v3, v3I);
                        u.Move(AStarPath);
                    }
                }
            }
        }
    }
}

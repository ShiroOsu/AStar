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
        public delegate void Action<T, T1>(ref T ref1, ref T1 ref2);
        
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

                        var ref1 = hit.point;
                        var ref2 = u.GetPosition();
                        CalculateAStarPath?.Invoke(ref ref1, ref ref2);
                    }
                }
            }
        }
    }
}

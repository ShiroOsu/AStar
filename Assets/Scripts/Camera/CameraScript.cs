using UnityEngine;

namespace Camera
{
    public class CameraScript : MonoBehaviour
    {
        private Vector3 m_ForwardVector;
        private Transform m_ThisTransform;

        private void Awake()
        {
            m_ThisTransform = transform;
        }

        private void Update()
        {
            MoveCamera();
        }


        private void CameraDirection()
        {
            var directionX = Input.GetAxisRaw("Horizontal");
            var directionZ = Input.GetAxisRaw("Vertical");
            var dir = new Vector3(directionX, 0f, directionZ);
            m_ForwardVector.x = dir.x;
            m_ForwardVector.z = dir.z;
        }

        private void MoveCamera()
        {
            CameraDirection();
            var forward = m_ThisTransform.rotation * m_ForwardVector;
            forward.y = 0f;
            m_ThisTransform.position += forward.normalized * (50f * Time.deltaTime);
        }
    }
}
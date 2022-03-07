using AStar;
using Interfaces;
using UnityEngine;

namespace Units
{
    public class UnitScript : MonoBehaviour, IUnit
    {
        public void Move(Vector3 destination)
        {
            
        }

        public Vector3Int GetPosition()
        {
            return Extensions.Vector3ToVector3Int(gameObject.transform.position);
        }
    }
}

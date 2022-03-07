using UnityEngine;

namespace Interfaces
{
    public interface IUnit
    {
        public void Move(Vector3 destination);
        public Vector3Int GetPosition();
    }
}

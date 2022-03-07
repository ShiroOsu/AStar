using System.Collections.Generic;
using AStar;
using UnityEngine;

namespace Interfaces
{
    public interface IUnit
    {
        public void Move(IReadOnlyList<Cell> path);
        public Vector3Int GetPosition();
    }
}

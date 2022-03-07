using System.Collections;
using System.Collections.Generic;
using AStar;
using Interfaces;
using UnityEngine;

namespace Units
{
    public class UnitScript : MonoBehaviour, IUnit
    {
        public float moveSpeed;
        private Coroutine m_CurrentPath;
        private int m_PathIndex;
        private Vector3 m_EndPos;
        private Vector3 m_NextPos;
        
        public void Move(IReadOnlyList<Cell> path)
        {
            if (m_CurrentPath != null)
            {
                StopCoroutine(m_CurrentPath);
            }

            InternalImp(path);
        }

        private void InternalImp(IReadOnlyList<Cell> path)
        {
            m_EndPos = path[^1].WorldPos;
            m_NextPos = path[0].WorldPos;
            m_CurrentPath = StartCoroutine(GoToNextDest(path));
        }

        private IEnumerator GoToNextDest(IReadOnlyList<Cell> path)
        {
            while (transform.position != m_EndPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_NextPos, 
                    moveSpeed * Time.deltaTime);
                
                if (transform.position == m_NextPos)
                {
                    m_PathIndex++;
                    if (m_PathIndex > path.Count - 1)
                    {
                        break;
                    }

                    m_NextPos = path[m_PathIndex].WorldPos;
                }
                yield return null;
            }
        }

        public Vector3Int GetPosition()
        {
            return Extensions.Vector3ToVector3Int(gameObject.transform.position);
        }
    }
}
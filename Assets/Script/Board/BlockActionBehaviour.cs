using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Scripable;
using Puzzle.Controller;

namespace Puzzle.Board
{
    public class BlockActionBehaviour : MonoBehaviour
    {
        public bool isMoving { get; set; }
        Queue<Vector3> movementQueue = new Queue<Vector3>();

        public void MoveDrop(Vector2 vtDropDistance)
        {
            movementQueue.Enqueue(new Vector3(vtDropDistance.x, vtDropDistance.y, 1));

            if(!isMoving)
            {
                StartCoroutine(DoActionMoveDrop());
            }
        }

        IEnumerator DoActionMoveDrop(float acc = 1.0f)
        {
            isMoving = true;

            while(movementQueue.Count>0)
            {
                Vector2 vtDestination = movementQueue.Dequeue();

                float duration = Core.Constants.DROP_TIME;
                yield return CoStartDropSmooth(vtDestination, duration * acc);
            }

            isMoving = false;
            yield break;
        }

        IEnumerator CoStartDropSmooth(Vector2 vtDropDistance, float duration)
        {
            Vector3 to = new Vector3(transform.position.x + vtDropDistance.x, transform.position.y - vtDropDistance.y, transform.position.z);
            yield return Action2D.MoveTo(transform, to, duration);
        }
    }
}
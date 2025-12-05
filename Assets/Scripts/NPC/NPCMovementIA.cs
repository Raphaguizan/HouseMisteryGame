using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Guizan.NPC
{
    [RequireComponent(typeof(NPCMovementController))]
    [RequireComponent(typeof(NPCController))]
    public class NPCMovementIA : MonoBehaviour
    {
        [SerializeField, MinMaxSlider(0f, 60f)]
        private Vector2 randomWaitTimeToMove;

        private float currentY;
        private Vector2 currentXBounds;
        private NPCController controller;
        private NPCMovementController MovementController;
        private bool inRoom = true;

        private IEnumerator Start()
        {
            MovementController = GetComponent<NPCMovementController>();
            controller = GetComponent<NPCController>();

            controller.RoomChanged += RoomChangeListener;

            yield return new WaitForSecondsRealtime(Random.Range(randomWaitTimeToMove.x, randomWaitTimeToMove.y));
            MoveInRoom();
        }

        private void RoomChangeListener()
        {
            inRoom = false;
        }
        private void MoveInRoom()
        {
            var roomBounds = controller.GetRoomBounds();
            currentY = roomBounds.Item2;
            currentXBounds = roomBounds.Item1;
            StartCoroutine(MovementLoop());
        }

        private IEnumerator MovementLoop()
        {
            inRoom = true;
            while (inRoom)
            {
                yield return new WaitForSecondsRealtime(Random.Range(randomWaitTimeToMove.x, randomWaitTimeToMove.y));
                MovementController.MoveToPosition(new Vector2(GenerateRandomXPos(currentXBounds), currentY));
                yield return new WaitWhile(() => MovementController.IsMoving());
            }
        }

        private float GenerateRandomXPos(Vector2 minMax)
        {
            return Random.Range(minMax.x+1, minMax.y-1);
        }

        private void OnDestroy()
        {
            controller.RoomChanged -= RoomChangeListener;
        }
    }
}
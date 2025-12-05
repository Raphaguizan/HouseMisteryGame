using Guizan.House;
using Guizan.House.Room;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Guizan.NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCMovementController : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        private NavMeshAgent agent;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        [Button]
        public void MoveToPosition(Nullable<Vector2> targetPos = null)
        {
            if (!agent.isOnNavMesh)
            {
                Debug.LogWarning("O agente não está em uma navMesh.");
                return;
            }
            Vector3 destination = targetPos == null?target.position : targetPos.Value;
            agent.SetDestination(destination);
        }

        [Button]
        public void StopMovement()
        {
            agent.isStopped = true;
        }

        public void MoveToRoom(Vector2Int pos)
        {
            MoveToRoom(pos.x, pos.y);
        }
        public void MoveToRoom(int x, int y)
        {
            HouseManager manager = FindAnyObjectByType<HouseManager>();
            if (manager == null)
                return;
            Transform room = manager.GetRooms().Find(r => r.CurrentPos.x == x && r.CurrentPos.y == y).transform;
            MoveToPosition(room.position);
        }

        public void MoveToRoom (RoomType type)
        {
            HouseManager manager = FindAnyObjectByType<HouseManager>();
            if (manager == null)
                return;
            Transform room = manager.GetRooms().Find(r => r.RoomType == type).transform;
            MoveToPosition(room.position);
        }

        public bool IsMoving()
        {
            return agent.remainingDistance > agent.stoppingDistance;
        }
    }
}
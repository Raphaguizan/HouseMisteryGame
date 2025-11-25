using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Guizan.NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCMovement : MonoBehaviour
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
    }
}
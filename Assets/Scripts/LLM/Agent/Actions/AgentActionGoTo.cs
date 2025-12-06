using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizan.House.Room;
using System;
using Guizan.NPC;

namespace Guizan.LLM.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToAction", menuName = "NPC/Actions/GoTo")]
    public class AgentActionGoTo : AgentActionBinder
    {
        [SerializeField]
        private AgentActionEndConversation endConversation;
        public override void MakeAction(Dictionary<string, object> Parameters)
        {
            GameObject npcObject = Parameters["NPC"] as GameObject;

            if (Parameters["Room"] is string roomString && npcObject != null)
            {
                if (Enum.TryParse<RoomType>(roomString, true, out RoomType roomType))
                {
                    if (!npcObject.TryGetComponent<NPCMovementController>(out var myNPCPrefab))
                    {
                        Debug.LogError("O GameObject fornecido não possui um componente NPCMovementController.");
                        return;
                    }
                    if (endConversation != null)
                    {
                        endConversation.MakeAction(Parameters);
                    }
                    Debug.Log($"Movendo NPC para a sala: {roomType}");
                    myNPCPrefab.MoveToRoom(roomType);
                }
            }
        }
    }
}
using Guizan.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    [CreateAssetMenu(fileName ="EndConversationAction", menuName ="NPC/Actions/EndConversation")]
    public class AgentActionEndConversation : AgentActionBinder
    {
        public override void MakeAction(Dictionary<string, object> Parameters)
        {
            Debug.Log("Conversa finalizada");
            DialogManager.FinalizeConversation();
        }
    }
}
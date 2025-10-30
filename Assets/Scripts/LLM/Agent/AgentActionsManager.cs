using Guizan.LLM.Agent.Actions;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class AgentActionsManager : MonoBehaviour
    {
        [SerializeField]
        private List<AgentAction> actionSubscribe = new();


        public void Subscribe(AgentAction action)
        {
            actionSubscribe.Add(action);
        }

        public void MakeAction(LLMAction action)
        {
            Debug.Log("chamou a ação " + action.Type);
            actionSubscribe.Find(script => script.Type.Equals(action.Type))?.MakeAction(action.Parameters);
        }

        private void OnValidate()
        {
            if (HasEqual())
            {
                Debug.LogError("Exitem dois elementos na lista de scripts com o mesmo tipo de ação");
            }
        }

        private bool HasEqual()
        {
            if (actionSubscribe.Count <= 1)
                return false;

            bool resp = false;
            for (int i = 0; i < actionSubscribe.Count; i++)
            {
                if (actionSubscribe.Exists(item => !actionSubscribe[i].Equals(item) && item.Type == actionSubscribe[i].Type))
                    resp = true;
            }
            return resp;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    public class AgentAction : MonoBehaviour, IAgentAction
    {
        [SerializeField]
        protected string _myType;

        public virtual string Type => _myType;

        protected virtual void Awake()
        {
            AgentActionsManager ationsManager = GetComponent<AgentActionsManager>();
            if (ationsManager == null)
            {
                ationsManager = GetComponentInParent<AgentActionsManager>();
            }
            if (ationsManager == null)
                return;
            ationsManager.Subscribe(this);
        }

        public virtual void MakeAction(Dictionary<string, object> Parameters)
        {
            string dictonaryText = "";
            foreach(object item in Parameters)
            {
                dictonaryText += item.ToString()+", ";
            }
            Debug.Log($"Making action {Type} with parans {dictonaryText}");
        }
    }
}
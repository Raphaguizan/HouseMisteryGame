using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    public class AgentActionBinder : ScriptableObject, IAgentAction
    {
        [SerializeField]
        protected string type;
        public virtual string Type => type;

        public virtual void MakeAction(Dictionary<string, object> Parameters) { }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    public class AgentActionBinder : ScriptableObject, IAgentAction
    {
        public string Type => throw new System.NotImplementedException();

        public virtual void MakeAction(Dictionary<string, object> Parameters) { }
    }
}
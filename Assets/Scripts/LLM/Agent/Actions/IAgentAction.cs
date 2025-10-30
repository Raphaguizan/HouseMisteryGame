using System.Collections.Generic;

namespace Guizan.LLM.Agent.Actions
{
    public interface IAgentAction
    {
        public string Type { get; }
        public void MakeAction(Dictionary<string, object> Parameters);
    }
}
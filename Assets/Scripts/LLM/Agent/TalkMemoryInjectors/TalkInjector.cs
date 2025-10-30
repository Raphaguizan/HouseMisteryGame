using System.Runtime.Serialization;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class TalkInjector : MonoBehaviour
    {
        protected virtual void Awake()
        {
            AgentTalkMemoryInjection injectManager = GetComponent<AgentTalkMemoryInjection>();
            if (injectManager == null)
            {
                injectManager = GetComponentInParent<AgentTalkMemoryInjection>();
            }
            if (injectManager == null)
                return;
            injectManager.Subscribe(this);
        }

        public virtual string GetTextToInject()
        {
            return "";
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    [RequireComponent(typeof(AgentTalkManager))]
    public class AgentTalkMemoryInjection : MonoBehaviour
    {
        [SerializeField]
        private List<TalkInjector> injectors;

        AgentTalkManager talkManager;
        private void Awake()
        {
            injectors = new();
            talkManager = GetComponent<AgentTalkManager>();
            talkManager.ConversationStartCallBack.AddListener(InjectMemory);
        }

        public void Subscribe(TalkInjector injector)
        {
            injectors.Add(injector);
        }

        public void InjectMemory()
        {
            foreach (TalkInjector injector in injectors)
            {
                talkManager.AddMemory(new(MessageRole.system, injector.GetTextToInject()));
            }
        }

        private void OnDestroy()
        {
            talkManager.ConversationStartCallBack.RemoveListener(InjectMemory);
        }
    }
}
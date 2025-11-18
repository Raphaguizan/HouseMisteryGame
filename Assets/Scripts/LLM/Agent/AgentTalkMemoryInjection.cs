
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    [RequireComponent(typeof(AgentTalkManager))]
    public class AgentTalkMemoryInjection : MonoBehaviour
    {
        [SerializeField]
        private List<TalkInjectorBinder> injectors;

        AgentTalkManager talkManager;
        private void Awake()
        {
            injectors = new();
            talkManager = GetComponent<AgentTalkManager>();
            talkManager.ConversationStartCallBack.AddListener(InjectMemory);
        }

        public void SetList(List<TalkInjectorBinder> newInjectors)
        {
            injectors.Clear();
            injectors = newInjectors;
        }

        public void Subscribe(TalkInjectorBinder injector)
        {
            injectors.Add(injector);
        }

        public void InjectMemory()
        {
            foreach (TalkInjectorBinder injector in injectors)
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
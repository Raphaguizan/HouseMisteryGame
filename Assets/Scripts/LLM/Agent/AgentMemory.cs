using Guizan.LLM;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    [CreateAssetMenu(fileName ="newAgentMemory", menuName ="LLM/Memory")]
    public class AgentMemory : ScriptableObject
    {
        [SerializeField]
        private string npcID;
        [SerializeField]
        private List<Message> memory;

        public List<Message> Memory => memory;
        public string ID => npcID;


        public void SetMemory(List<Message> newMemory)
        {
            memory = newMemory;
        }
        public void AddMemory(Message newMessage)
        {
            if (AlreadyInMemory(newMessage.content) && !newMessage.CompareRole(MessageRole.user))
                return;

            memory.Add(newMessage);
        }

        public void AddMemory(Message newMessage, int index)
        {
            if (AlreadyInMemory(newMessage.content) && !newMessage.CompareRole(MessageRole.user))
                return;

            memory.Insert(index, newMessage);
        }

        private bool AlreadyInMemory(string test)
        {
            return memory.Exists((i) => i.content.Equals(test));
        }

        public void ResetMemories()
        {
            memory.Clear();
        }
    }
}
using Guizan.LLM;
using Guizan.LLM.Agent;
using Guizan.LLM.Agent.Actions;
using Guizan.LLM.Embedding;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.NPC
{
    [CreateAssetMenu(menuName = "NPC/configs", fileName = "newNPCConfig")]
    public class NPCConfigs : ScriptableObject
    {
        [SerializeField]
        private List<Sprite> dialogEmoticonsImages;
        [SerializeField, Expandable]
        private AgentMemory agentLLMMemory;
        [SerializeField]
        private List<PermanentMemory> permanentMemories;
        [SerializeField]
        private List<FileEmbedding> fileEmbeddings;
        [SerializeField, Expandable]
        private List<TalkInjectorBinder> talkInjectors;
        [SerializeField, Expandable]
        private List<AgentActionBinder> agentActions;
        [SerializeField]
        private GameObject npcPrefabObject;

        public List<Sprite> DialogEmoticonsImg => dialogEmoticonsImages;
        public AgentMemory AgentLLMMemory => agentLLMMemory;
        public List<PermanentMemory> PermanentMemories => permanentMemories;
        public List<FileEmbedding> FileEmbeddings => fileEmbeddings;
        public List<TalkInjectorBinder> TalkInjectors => talkInjectors;
        public List<AgentActionBinder> AgentActions => agentActions;
        public GameObject NpcPrefab => npcPrefabObject;

        public void SetNPCPrefab(GameObject newPrefab)
        {
            npcPrefabObject = newPrefab;
        }
    }
}
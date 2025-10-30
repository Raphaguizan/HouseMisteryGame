using Guizan.LLM;
using Guizan.LLM.Agent;
using Guizan.LLM.Embedding;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.NPC
{
    [CreateAssetMenu(menuName ="NPC/configs", fileName ="newNPCConfig")]
    public class NPCConfigs : ScriptableObject
    {
        public Sprite dialogImage;
        [Expandable]
        public AgentMemory agentLLMMemory;
        public List<PermanentMemory> permanentMemories;
        public List<FileEmbedding> fileEmbeddings;
    }
}
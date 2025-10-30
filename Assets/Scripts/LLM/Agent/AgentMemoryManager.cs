using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

namespace Guizan.LLM.Agent
{
    public class AgentMemoryManager : MonoBehaviour
    {
        [SerializeField, Expandable]
        private AgentMemory myMemory;

        [Space, SerializeField]
        private List<PermanentMemory> permanentMemoriesList;

        public List<Message> Memory => myMemory.Memory;

        private void Start()
        {
            LoadMemory();
        }

        public void SetMemory(List<Message> newMemory)
        {
            myMemory.SetMemory(newMemory);
        }

        public void AddMemory(Message message)
        {
            myMemory.AddMemory(message);
            VerifyMemoryLenght();
        }

        public void AddMemory(Message message, int index)
        {
            myMemory.AddMemory(message, index);
        }

        public void AddMemory(MessageRole role, string content)
        {
            AddMemory(new(role, content));
        }

        private void InjectPermanentMemory()
        {
            for (int i = 0; i < permanentMemoriesList.Count; i++)
            {
                AddMemory(permanentMemoriesList[i].Message, i);
            }
        }

        [SerializeField, Foldout("Memory Sumarize")]
        private int memoryTotalLenght = 50;

        [SerializeField, ResizableTextArea, Foldout("Memory Sumarize")]
        private string sumaryPrompt = "Você está interagindo com um jogador por meio de um personagem (NPC) em um jogo. O que foi conversado até agora não será enviado novamente. A partir desta mensagem, quero que você gere um resumo conciso da conversa anterior.\r\n\r\nEsse resumo deve servir como uma memória para o NPC, contendo apenas os fatos importantes, intenções, decisões ou sentimentos relevantes que o jogador demonstrou.\r\n\r\nIgnore cumprimentos, piadas ou partes irrelevantes.\r\n\r\nEscreva o resumo como se fosse uma nota pessoal do NPC para lembrar o que aconteceu até agora. Use frases curtas, diretas, no estilo de tópicos.\r\n\r\nExemplo:\r\n- O jogador contou que foi expulso da colônia Aurora por questionar a liderança.\r\n- Está procurando pistas sobre um artefato chamado Prisma Sombrio.\r\n- Pediu ajuda para encontrar um engenheiro chamado Drax.\r\n\r\nAgora, por favor, resuma a conversa até este ponto.\r\n";

        private Message lastAssistantMessage = null;

        private void VerifyMemoryLenght()
        {
            if (Memory[^1].role.Equals("user"))
                return;

            if (Memory.Count >= memoryTotalLenght)
                SumarizeMemory();
        }

        public void ResetMemory()
        {
            myMemory.ResetMemories();
            InjectPermanentMemory();
        }

        public void MakeTalkSumary(List<Message> talkMemory, Action<Message> onSumaryCallback)
        {
            talkMemory.Add(new(MessageRole.user, sumaryPrompt));
            GroqLLM.SendMessageToLLM(talkMemory, (response) => onSumaryCallback?.Invoke(new(MessageRole.system, response.GetFullText())));
        }

        /// <summary>
        /// Faz um resumo da conversa sem esperar o limite máximo de mensagens.
        /// Quando o jogador parar de interagir com o NPC ele saber que isso aconteceu e não apenas
        /// continuar a conversa como se nada tivesse acontecido.
        /// </summary>
        /// <param name="resumeMessage">Mensagem do sistema antes de fazer o resumo. exemplo: "O jogador foi embora e acabou a conversa."</param>
        public void MakeMemorySumary(string resumeMessage)
        {
            AddMemory(MessageRole.system, resumeMessage);
            SumarizeMemory(false);
        }

        private void SumarizeMemory(bool keepLastMessage = true)
        {
            if (keepLastMessage)
                lastAssistantMessage = GetLastAssistantMessage();

            AddMemory(MessageRole.user, sumaryPrompt);
            GroqLLM.SendMessageToLLM(Memory, ReceiveSumary);
        }

        private void ReceiveSumary(ResponseLLM response)
        {
            myMemory.ResetMemories();
            AddMemory(new(MessageRole.system, response.GetFullText()));

            if (lastAssistantMessage != null)
                AddMemory(lastAssistantMessage);

            lastAssistantMessage = null;
            InjectPermanentMemory();
        }

        private Message GetLastAssistantMessage(List<Message> memory = null)
        {
            memory ??= Memory;

            for (int i = memory.Count - 1; i >= 0; i--)
            {
                if (memory[i].role.Equals(MessageRole.assistant))
                    return memory[i];
            }
            return null;
        }

        #region Save
        
        [Button]
        public void ResetSavedMemory()
        {
            AgentJSONSaver<List<Message>>.ClearJSON(myMemory.ID, SavePathFolder.npc_Memory);
            ResetMemory();
        }

        private void SaveMemory()
        {
            AgentJSONSaver<List<Message>>.SaveJSON(myMemory.ID, Memory, SavePathFolder.npc_Memory);
        }

        private void LoadMemory()
        {
            var newMemory = AgentJSONSaver<List<Message>>.LoadJSON(myMemory.ID, SavePathFolder.npc_Memory);
            if (newMemory == default)
                InjectPermanentMemory();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
                SaveMemory();
        }

        private void OnDestroy()
        {
            SaveMemory();
        }
        #endregion
    }
}
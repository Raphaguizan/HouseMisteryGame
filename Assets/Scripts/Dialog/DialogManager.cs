using Guizan.LLM.Agent;
using Guizan.LLM.Embedding;
using Guizan.NPC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Guizan.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField]
        private AgentDialogManager agentManager;

        [SerializeField]
        private AgentTalkManager talkManager;

        [SerializeField]
        private TMP_InputField textArea;

        [SerializeField]
        private Button sendButton;
        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private GameObject graphics; 

        private void OnEnable()
        {
            sendButton.onClick.AddListener(SendMessage);
            exitButton.onClick.AddListener(EndConversation);
            graphics.SetActive(false);
        }

        private void Update()
        {
            if (!graphics.activeInHierarchy)
                return;
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
                SendMessage();
        }

        public void InitializeDialog(NPCConfigs npc)
        {
            if (npc == null)
                return;

            talkManager.StartConversation();
            graphics.SetActive(true);

            GameObject agentGO = talkManager.gameObject;
            if (agentGO.TryGetComponent<AgentMemoryManager>(out AgentMemoryManager memoryManager))
                memoryManager.SetMemory(npc.agentLLMMemory, npc.permanentMemories);

            if (agentGO.TryGetComponent<AgentEmbeddingManager>(out AgentEmbeddingManager fileEmbeddings))
                fileEmbeddings.SetFileEmbeddings(npc.fileEmbeddings);

            agentManager.InitializeDialog(npc.dialogImage);
            agentManager.ReceiveAnswer(new() { "Olá, como posso ajudar?" });
        }

        private void SendMessage()
        {
            if (textArea.text.Equals(string.Empty))
                return;

            talkManager.SendMessage(textArea.text, callback: (pages) => agentManager.ReceiveAnswer(pages));
            textArea.text = string.Empty;
        }

        public void EndConversation()
        {
            talkManager.EndConversation();
            graphics.SetActive(false);
        }

        private void OnDisable()
        {
            sendButton.onClick.RemoveListener(SendMessage);
            exitButton.onClick.RemoveListener(EndConversation);
        }
    }
}
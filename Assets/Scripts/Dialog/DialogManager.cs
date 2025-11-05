using Guizan.LLM.Agent;
using Guizan.LLM.Embedding;
using Guizan.NPC;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Game.Util;

namespace Guizan.Dialog
{
    public class DialogManager : Singleton<DialogManager>
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

        public static bool Initialized => Instance.graphics.activeInHierarchy;

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

        public static void InitializeDialog(NPCConfigs npc, string initialMessage = "Olá!")
        {
            if (npc == null || Initialized)
                return;

            Instance.talkManager.StartConversation();
            Instance.graphics.SetActive(true);

            GameObject agentGO = Instance.talkManager.gameObject;
            if (agentGO.TryGetComponent<AgentMemoryManager>(out AgentMemoryManager memoryManager))
                memoryManager.SetMemory(npc.agentLLMMemory, npc.permanentMemories);

            if (agentGO.TryGetComponent<AgentEmbeddingManager>(out AgentEmbeddingManager fileEmbeddings))
                fileEmbeddings.SetFileEmbeddings(npc.fileEmbeddings);

            Instance.agentManager.InitializeDialog(npc.dialogImage);
            //Instance.agentManager.ReceiveAnswer(new() { "Olá, como posso ajudar?" });
            Instance.talkManager.SendMessage(initialMessage, callback: (pages) => Instance.agentManager.ReceiveAnswer(pages));
        }

        private void SendMessage()
        {
            if (textArea.text.Equals(string.Empty))
                return;

            talkManager.SendMessage(textArea.text, callback: (pages) => agentManager.ReceiveAnswer(pages));
            textArea.text = string.Empty;
        }

        public static void EndConversation()
        {
            if (!Initialized)
                return;

            Instance.talkManager.EndConversation();
            Instance.graphics.SetActive(false);
        }

        private void OnDisable()
        {
            sendButton.onClick.RemoveListener(SendMessage);
            exitButton.onClick.RemoveListener(EndConversation);
        }
    }
}
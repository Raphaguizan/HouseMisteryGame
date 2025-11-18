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

        public static bool Initialized => Instance.graphics.activeInHierarchy || Instance.talkManager.InConversation;

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

            Debug.Log("Iniciou a conversa");
            Instance.graphics.SetActive(true);

            GameObject agentGO = Instance.talkManager.gameObject;
            //Adiciona as memórias permanentes
            if (agentGO.TryGetComponent<AgentMemoryManager>(out AgentMemoryManager memoryManager))
                memoryManager.SetMemory(npc.AgentLLMMemory, npc.PermanentMemories);

            //Adiciona os arquivos de embeddings
            if (agentGO.TryGetComponent<AgentEmbeddingManager>(out AgentEmbeddingManager fileEmbeddings))
                fileEmbeddings.SetFileEmbeddings(npc.FileEmbeddings);

            //Adiciona os TalkInjectors
            if (agentGO.TryGetComponent<AgentTalkMemoryInjection>(out AgentTalkMemoryInjection injectors))
                injectors.SetList(npc.TalkInjectors);

            //Adiciona os AgentsActions
            if (agentGO.TryGetComponent<AgentActionsManager>(out AgentActionsManager actionsManager))
                actionsManager.SetList(npc.AgentActions);

            Instance.talkManager.StartConversation();

            Instance.agentManager.InitializeDialog(npc.DialogEmoticonsImg);
            //Instance.agentManager.ReceiveAnswer(new() { "Olá, como posso ajudar?" });
            Instance.talkManager.SendMessage(initialMessage, callback: (pages, emoticons) => Instance.agentManager.ReceiveAnswer(pages, emoticons));
        }

        private void SendMessage()
        {
            if (textArea.text.Equals(string.Empty))
                return;

            agentManager.WaitingForAnswer();
            talkManager.SendMessage(textArea.text, callback: (pages, emoticons) => agentManager.ReceiveAnswer(pages, emoticons));
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
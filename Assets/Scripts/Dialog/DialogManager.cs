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

        private bool conversationFinalized = false;

        //private bool Initialized;

        public static bool Initialized = false;

        private void OnEnable()
        {
            sendButton.onClick.AddListener(SendButton);
            exitButton.onClick.AddListener(EndConversation);
            graphics.SetActive(false);
            textArea.text = string.Empty;
        }

        private void Update()
        {
            if (!Initialized)
                return;
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
                SendButton();

            CheckSendButton();
        }

        public static void InitializeDialog(NPCConfigs npc, string initialMessage = "Ol�!")
        {
            if (npc == null || Initialized)
                return;

            Debug.Log("Iniciou a conversa");
            Instance.graphics.SetActive(true);

            GameObject agentGO = Instance.talkManager.gameObject;
            //Adiciona as mem�rias permanentes
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
                actionsManager.SetList(npc: npc.NpcPrefab);

            Instance.talkManager.StartConversation();

            Instance.agentManager.InitializeDialog(npc.DialogEmoticonsImg);
            Instance.talkManager.SendMessage(initialMessage, callback: (pages, emoticons) => {
                    Initialized = true;
                    Instance.agentManager.ReceiveAnswer(pages, emoticons);
                });
        }

        private void CheckSendButton()
        {

            // Ajusta o inputfield para terminar a conver�a
            if (agentManager.InLastPage && textArea.enabled == false && conversationFinalized)
            {
                sendButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sair";
                var placeholderText = textArea.placeholder as TMP_Text;
                textArea.text = string.Empty;
                if (placeholderText != null)
                {
                    placeholderText.text = string.Empty;
                }
                textArea.enabled = false;
                return;
            }

            // Ajusta o inputfield para enviar a mensagem
            if (agentManager.InLastPage && textArea.enabled == false)
            {
                sendButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enviar";
                var placeholderText = textArea.placeholder as TMP_Text;
                textArea.enabled = true;
                if (placeholderText != null)
                {
                    placeholderText.text = "Escreva sua mensagem.";
                }
                textArea.Select();
                return;
            }

            // Ajusta o inputfield para antes da ultima p�gina (n�o pode enviar mensagem)
            if (!agentManager.InLastPage && textArea.enabled == true)
            {
                sendButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pr�xima P�gina";
                var placeholderText = textArea.placeholder as TMP_Text;
                textArea.text = string.Empty;
                if (placeholderText != null)
                {
                    placeholderText.text = string.Empty;
                }
                textArea.enabled = false;
            }
        }

        private void SendButton()
        {
            if (!agentManager.InLastPage)
            {
                agentManager.NextPage();
                return;
            }
            if (conversationFinalized)
            {
                EndConversation();
                return;
            }
            if (textArea.text.Equals(string.Empty))
                return;

            agentManager.WaitingForAnswer();
            talkManager.SendMessage(textArea.text, callback: (pages, emoticons) => agentManager.ReceiveAnswer(pages, emoticons));
            textArea.text = string.Empty;
            textArea.enabled = false;
        }

        public static void FinalizeConversation()
        {
            Instance.conversationFinalized = true;
        }

        public static void EndConversation()
        {
            if (!Initialized)
                return;

            Instance.talkManager.EndConversation();
            Instance.graphics.SetActive(false);
            Instance.conversationFinalized = false;
            Instance.textArea.enabled = false;
            Initialized = false;
        }

        private void OnDisable()
        {
            sendButton.onClick.RemoveListener(SendButton);
            exitButton.onClick.RemoveListener(EndConversation);
        }
    }
}
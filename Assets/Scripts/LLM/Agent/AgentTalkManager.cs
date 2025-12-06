using Guizan.LLM.Embedding;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Guizan.LLM.Agent
{

    public class AgentTalkManager : MonoBehaviour
    {
        [SerializeField, ResizableTextArea, Tooltip("Frase que ser� enviada caso ocorra algum erroe o request n�o retornar Success.")]
        private string defaultMessage = "Error";


        [SerializeField]
        private List<Message> talkMemory;

        private AgentMemoryManager memoryManager;
        private AgentEmbeddingManager embedding;
        private AgentActionsManager actionsManager;

        private Action<List<string>, List<AgentEmoticons>> responseCallBack;
        
        private bool inConversation;

        [Foldout("Events")]
        public UnityEvent ConversationStartCallBack;
        [Foldout("Events")]
        public UnityEvent<Message> ConversationEndCallBack;

        [ShowNativeProperty]
        public bool InConversation => inConversation;

        public string DefaultMessage => defaultMessage;
        private void Awake()
        {
            embedding = GetComponent<AgentEmbeddingManager>();
            memoryManager = GetComponent<AgentMemoryManager>();
            actionsManager = GetComponent<AgentActionsManager>();
            inConversation = false;
            responseCallBack = null;
        }

        public void AddMemory(Message newMemory)
        {
            talkMemory.Add(newMemory);
        }

        public void StartConversation()
        {
            talkMemory.Clear();
            ConversationStartCallBack?.Invoke();
            inConversation = true;
        }

        public void EndConversation()
        {
            if (!inConversation || memoryManager == null || talkMemory.Count == 0)
                return;

            memoryManager.MakeTalkSumary(talkMemory, (sumarymessage) =>
            {
                if(!sumarymessage.content.Trim().Equals("[NONE]"))
                    memoryManager.AddMemory(sumarymessage);

                ConversationEndCallBack?.Invoke(sumarymessage);
            });
            inConversation = false;
        }

        public void SendMessage(string message, MessageRole role = MessageRole.user, Action<List<string>, List<AgentEmoticons>> callback = null)
        {
            if (!inConversation)
                StartConversation();

            responseCallBack = callback;
            Message newMessage = new(role, message);

            talkMemory.Add(newMessage);
            List<Message> memoriesMessages = talkMemory;

            if (memoryManager != null)
            {
                memoriesMessages = memoryManager.Memory.Concat(talkMemory).ToList();
            }

            if (embedding != null)
            {
                embedding.TestEmbedding(message, (msg) => {
                    if (msg != null && msg.Count > 0)
                    {
                        for (int i = 0; i < msg.Count; i++)
                        {
                            if (!talkMemory.Exists(a => a.content.Equals(msg[i].content)))
                                talkMemory.Add(msg[i]);
                            memoriesMessages.Add(msg[i]);
                        }
                    }
                    GroqLLM.SendMessageToLLM(memoriesMessages, ReceiveAnswer);
                });
                return;
            }

            GroqLLM.SendMessageToLLM(memoriesMessages, ReceiveAnswer);
        }

        private void ReceiveAnswer(ResponseLLM responseLLM)
        {
            Debug.Log("Receive:\n"+responseLLM.FullResponse);
            TalkResponseLLM response = new(responseLLM);

            List<string> pages = new();
            List<AgentEmoticons> emoticons = new();
            if(response.type == ResponseType.Error)
            {
                Debug.LogError(defaultMessage);  
                pages.Add(defaultMessage);
                response.Action.Type = "end_conversation";
                actionsManager.MakeAction(response.Action);
            } 
            else 
            {
                talkMemory.Add(new(MessageRole.assistant, response.GetFullText()));
                pages = response.Pages;
                emoticons = response.Emoticons;
                if (actionsManager != null && response.Action.Type != "none")
                    actionsManager.MakeAction(response.Action);
            }

            responseCallBack?.Invoke(pages, emoticons);
            responseCallBack = null;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Guizan.LLM.Embedding;
using System.Linq;
using UnityEngine.Events;

namespace Guizan.LLM.Agent
{

    public class AgentTalkManager : MonoBehaviour
    {
        [SerializeField, ResizableTextArea, Tooltip("Frase que será enviada caso ocorra algum erroe o request não retornar Success.")]
        private string defaultMessage = "Error";


        [SerializeField]
        private List<Message> talkMemory;

        private AgentMemoryManager memoryManager;
        private AgentEmbeddingManager embedding;
        private AgentActionsManager actionsManager;

        private Action<List<string>> responseCallBack;
        
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
            inConversation = false;
            if (!inConversation || memoryManager == null || talkMemory.Count == 0)
                return;

            memoryManager.MakeTalkSumary(talkMemory, (sumarymessage) =>
            {
                memoryManager.AddMemory(sumarymessage);
                ConversationEndCallBack?.Invoke(sumarymessage);
            });
        }

        public void SendMessage(string message, MessageRole role = MessageRole.user, Action<List<string>> callback = null)
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

        private void ReceiveAnswer(ResponseLLM response)
        {
            Debug.Log("Receive:\n"+response.FullResponse);
            List<string> pages = new();
            if(response.type == ResponseType.Error)
            {
                Debug.LogError(defaultMessage);  
                pages.Add(defaultMessage);
                EndConversation();
            } 
            else 
            {
                talkMemory.Add(new(MessageRole.assistant, response.GetFullText()));
                pages = response.Pages;
                if (actionsManager != null && response.Action.Type != "none")
                    actionsManager.MakeAction(response.Action);
            }

            responseCallBack?.Invoke(pages);
            responseCallBack = null;
        }
    }
}
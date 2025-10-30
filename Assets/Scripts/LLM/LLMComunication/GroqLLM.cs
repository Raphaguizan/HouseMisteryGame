using Guizan.API;
using Guizan.LLM.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Game.Util;
using System;

namespace Guizan.LLM
{
    public class GroqLLM : Singleton<GroqLLM>
    {
        private const string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [SerializeField]
        private APIKeyConfig groqKey;

        [SerializeField, Expandable]
        private LLMConfigs defaulConfig;

        [Space, SerializeField]
        public static UnityEvent<ResponseLLM> ResponseEvent;

        private string apiKey;


        protected override void Awake()
        {
            base.Awake();
            apiKey = groqKey.Key;
            ResponseEvent = new();
        }
        //TODO: RETIRAR JEITO ANTIGO DE FAZER A CHAMADA
        //public static void SendMessageToLLM(AgentConfigs agent, Message userMessage, Action<ResponseLLM> onResponse = null)
        //{
        //    agent.AddMessage(userMessage);
        //    Instance.StartCoroutine(Instance.SendToGroq(agent, agent.GetRequest().messages,onResponse));
        //}

        public static void SendMessageToLLM(List<Message> messages, LLMConfigs configs,  Action<ResponseLLM> onResponse = null)
        {
            Instance.StartCoroutine(Instance.SendToGroq(configs, messages, onResponse));
        }

        public static void SendMessageToLLM(List<Message> messages, Action<ResponseLLM> onResponse = null)
        {
            if(Instance.defaulConfig == null)
            {
                Debug.LogError("Configuração padrão está vazia");
                return;
            }
            Instance.StartCoroutine(Instance.SendToGroq(Instance.defaulConfig, messages, onResponse));
        }

        IEnumerator SendToGroq(LLMConfigs configs, List<Message> messages, Action<ResponseLLM> onResponse = null)
        {
            //TODO: CORRIGIR A ENTRADA DE DADOS
            ConfigRequest newRequest = configs.GetRequest();
            newRequest.messages = messages;
            string jsonPayload = JsonUtility.ToJson(newRequest);

            // Corrige campos faltando do JsonUtility (como null e arrays)
            jsonPayload = jsonPayload.Replace("\"stop\":\"\"", "\"stop\":null");

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            ResponseLLM response = new();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string answerJson = request.downloadHandler.text;
                response.SetResponseData(answerJson.ExtractLLMMessage());
            }
            else
            {
                //Debug.LogError("Erro ao enviar para a Groq: " + request.error);

                response.SetResponseData(null, ResponseType.Error);
            }

            //Debug.Log(response);
            ResponseEvent.Invoke(response);
            onResponse?.Invoke(response);
        }
    }
}
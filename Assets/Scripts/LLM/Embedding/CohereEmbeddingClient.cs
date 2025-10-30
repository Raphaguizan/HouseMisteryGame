using Guizan.API;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Game.Util;
using NaughtyAttributes;

namespace Guizan.LLM.Embedding
{
    [Serializable]
    public class CohereEmbeddingRequest
    {
        public List<string> texts;
        public string model = "embed-multilingual-v3.0";
        public string input_type = "search_document";

        public CohereEmbeddingRequest(List<string> texts)
        {
            this.texts = texts;
        }
    }

    [Serializable]
    public class AgentEmbedding
    {
        [JsonProperty("chunks")]
        public List<string> TextChunks { get; set; }

        [JsonProperty("embeddings"), ShowNativeProperty]
        public List<List<float>> Embeddings { get; set; }

        public AgentEmbedding() 
        {
            Embeddings = null;
            TextChunks = null;
        }

        public AgentEmbedding(List<List<float>> embeddings, List<string> chunks)
        {
            this.Embeddings = embeddings;
            this.TextChunks = chunks;
        }

        public void SetEmpty()
        {
            TextChunks = null;
            Embeddings = null;
        }
        public bool IsEmpty()
        {
            return Embeddings == null || Embeddings.Count == 0;
        }

        public override string ToString()
        {
            string text = "";
            for (int i = 0; i < TextChunks.Count; i++)
            {
                text += " ";
                text += TextChunks[i];
            }
            if (Embeddings != null)
            {
                if (Embeddings.Count > 0)
                    text += "\nEmbedding index count = " + Embeddings.Count + " X " + Embeddings[0].Count;
                else
                    text += "no Embeddins!!!";
            }

            return text;
        }
    }

    public class CohereEmbeddingClient : Singleton<CohereEmbeddingClient>
    {
        [Header("Cohere API Settings")]
        [SerializeField] private APIKeyConfig apiKey;
        private const string endpoint = "https://api.cohere.ai/v1/embed";

        [Space]
        public static UnityEvent<AgentEmbedding, ResponseType> EmbedResponseEvent;

        protected override void Awake()
        {
            base.Awake();
            EmbedResponseEvent = new();
        }
        public static void RequestEmbeddings(string texts, Action<AgentEmbedding, ResponseType> onResponse = null)
        {
            List<string> chunks = new()
            {
                texts
            };

            RequestEmbeddings(chunks, onResponse);
        }
        public static void RequestEmbeddings(List<string> texts, Action<AgentEmbedding, ResponseType> onResponse = null)
        {
            Instance.StartCoroutine(Instance.RequestEmbeddingsCoroutine(texts, onResponse));
        }
        private IEnumerator RequestEmbeddingsCoroutine(List<string> texts, Action<AgentEmbedding, ResponseType> onResponse = null)
        {
            CohereEmbeddingRequest payload = new CohereEmbeddingRequest(texts);
            string json = JsonUtility.ToJson(payload);

            using UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey.Key}");

            yield return request.SendWebRequest();

            AgentEmbedding embeddingResponse = new();
            ResponseType type = ResponseType.Success;

            if (request.result == UnityWebRequest.Result.Success)
            {
                // A resposta é um JSON com "embeddings": [[...], [...]]
                string responseText = request.downloadHandler.text;
                embeddingResponse = JsonConvert.DeserializeObject<AgentEmbedding>(responseText);
            }
            else
            {
                type = ResponseType.Error;
                embeddingResponse.Embeddings = null;
            }

            embeddingResponse.TextChunks = texts;
            EmbedResponseEvent.Invoke(embeddingResponse, type);
            onResponse?.Invoke(embeddingResponse, type);
        }
    }
}

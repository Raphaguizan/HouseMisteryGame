using Guizan.LLM.Agent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Guizan.LLM.Embedding
{
    public class AgentEmbeddingManager : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        private float similarityAccept = .5f;

        //[SerializeField]
        //AgentMemoryManager memoryManager = null;

        [SerializeField]
        private List<FileEmbedding> FileEmbeddings;

        private List<Message> memoryResponse = new();

        public void TestEmbedding(string textToTest, Action<List<Message>> onEmbeddingResponse = null)
        {
            memoryResponse.Clear();
            StartCoroutine(WaitMemoryResponse(textToTest, onEmbeddingResponse));
        }

        private IEnumerator WaitMemoryResponse(string textToTest, Action<List<Message>> onEmbeddingResponse)
        {
            int count = FileEmbeddings.Count;
            MakeEmbedding(textToTest, (emb) =>
            {
                foreach (var embedding in FileEmbeddings)
                {
                    TestFileEmbeddingSimilarity(emb, embedding, () => count--);
                }
            });
            yield return new WaitWhile(() => count > 0);
            onEmbeddingResponse?.Invoke(memoryResponse);           
        }

        private void MakeEmbedding(string text, Action<AgentEmbedding> onCohereResponse)
        {
            CohereEmbeddingClient.RequestEmbeddings(text, (emb, type) => {
                AgentEmbedding resp = default;

                if (type == ResponseType.Success)
                    resp = emb;
                else
                    Debug.LogError("Erro na resposta do cohere: " + emb.ToString() + "  " + type.ToString());

                onCohereResponse.Invoke(resp);
            });
        }

        private void TestFileEmbeddingSimilarity(AgentEmbedding agentEmb, FileEmbedding fileEmb, Action callback = null)
        {
            try
            {
                fileEmb.GetEmbeddings((embeddings) =>
                {
                    var (index, score) = EmbeddingUtils.GetMostSimilarEmbedding(agentEmb.Embeddings[0], embeddings);
                    Debug.Log($"score : {score}  Index : {index}\n\ntexto:\n{agentEmb.TextChunks[0]}");
                    if (score > similarityAccept)
                    {
                        AddEmbeddingMemory(MakeSystemPrompt(embeddings, index));
                    }
                    callback?.Invoke();
                });
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                throw new Exception(e.Message);
            }

        }

        private Message MakeSystemPrompt(AgentEmbedding agentEmb, int chunkIndex)
        {
            string chunkText = agentEmb.TextChunks[chunkIndex];
            string prompt = $"Agora leve em consideração na hora de responder ao usuário essa história do personagem que você está interpertando:\n\"{chunkText}\"";
            return new Message(MessageRole.system, prompt); 
        }

        private void AddEmbeddingMemory(Message message)
        {
            memoryResponse.Add(message);
            /*if (memoryManager == null)
                return;

            memoryManager.AddMemory(message);*/
        }
    }
}
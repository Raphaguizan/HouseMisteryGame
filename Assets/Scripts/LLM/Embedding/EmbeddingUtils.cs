using Guizan.LLM.Embedding;
using System;
using System.Collections.Generic;

namespace Guizan.LLM.Embedding
{
    public static class EmbeddingUtils
    {
        /// <summary>
        /// Encontra o índice do embedding mais similar ao vetor de consulta, baseado na similaridade de cosseno.
        /// </summary>
        /// <param name="queryEmbedding">Embedding da consulta (ex: pergunta do jogador).</param>
        /// <param name="agentEmbedding">Objeto contendo lista de embeddings da memória do NPC.</param>
        /// <returns>
        /// Tupla contendo:
        /// - índice do embedding mais similar na lista,
        /// - valor da similaridade (entre -1 e 1, sendo 1 a máxima similaridade).
        /// </returns>
        /// <exception cref="Exception">Se a lista de embeddings estiver vazia.</exception>
        public static (int index, float score) GetMostSimilarEmbedding(List<float> queryEmbedding, AgentEmbedding agentEmbedding)
        {
            if (agentEmbedding.Embeddings == null || agentEmbedding.Embeddings.Count == 0)
            {
                throw new Exception("AgentEmbedding is empty.");
            }

            float maxScore = float.MinValue;
            int maxIndex = -1;

            // Percorre todos os embeddings para encontrar o mais similar
            for (int i = 0; i < agentEmbedding.Embeddings.Count; i++)
            {
                var npcEmbedding = agentEmbedding.Embeddings[i];
                float similarity = CosineSimilarity(queryEmbedding, npcEmbedding);

                if (similarity > maxScore)
                {
                    maxScore = similarity;
                    maxIndex = i;
                }
            }

            return (maxIndex, maxScore);
        }

        /// <summary>
        /// Calcula a similaridade de cosseno entre dois vetores.
        /// Valores próximos de 1 indicam alta similaridade, 0 indica vetores ortogonais.
        /// </summary>
        /// <param name="vecA">Primeiro vetor (lista de floats).</param>
        /// <param name="vecB">Segundo vetor (lista de floats).</param>
        /// <returns>Valor da similaridade (float entre -1 e 1).</returns>
        /// <exception cref="ArgumentException">Se os vetores não tiverem o mesmo tamanho.</exception>
        public static float CosineSimilarity(List<float> vecA, List<float> vecB)
        {
            if (vecA.Count != vecB.Count)
                throw new ArgumentException("Vectors must be of same length");

            float dot = 0f;
            float magA = 0f;
            float magB = 0f;

            // Calcula o produto escalar e magnitudes
            for (int i = 0; i < vecA.Count; i++)
            {
                dot += vecA[i] * vecB[i];
                magA += vecA[i] * vecA[i];
                magB += vecB[i] * vecB[i];
            }

            float denominator = (float)(Math.Sqrt(magA) * Math.Sqrt(magB));
            return (denominator == 0f) ? 0f : dot / denominator;
        }
    }
}
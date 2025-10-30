using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Guizan.LLM.Utils
{
    public static class LLMUtils
    {
        /// <summary>
        /// Extrai o conteúdo da mensagem retornada por um modelo LLM a partir de um JSON no formato da resposta da API.
        /// </summary>
        /// <param name="json">A string JSON contendo a resposta completa do modelo LLM (ex: resposta da API Groq/OpenAI).</param>
        /// <returns>O conteúdo da mensagem (campo "content") presente em choices[0].message.content, ou null se não for encontrado.</returns>

        public static string ExtractLLMMessage(this string json)
        {
            JObject obj = JObject.Parse(json);
            return (string)obj["choices"]?[0]?["message"]?["content"];
        }

        /// <summary>
        /// Divide o texto em chunks de até maxTokens (estimado), com overlap e sem cortar frases.
        /// </summary>
        /// <param name="text">Texto a ser dividido</param>
        /// <param name="maxTokens">Tamanho máximo por chunk</param>
        /// <param name="overlapTokens">Tokens do final do chunk anterior a repetir no seguinte</param>
        /// <returns>Lista de chunks com overlap</returns>
        public static List<string> SplitTextIntoChunksWithOverlap(this string text, int maxTokens = 500, int overlapTokens = 50)
        {
            var chunks = new List<string>();
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+"); // Divide em frases

            var currentChunk = new List<string>();
            var tokenCount = 0;

            for (int i = 0; i < sentences.Length; i++)
            {
                string sentence = sentences[i];
                int sentenceTokens = CountTokens(sentence);

                if (sentenceTokens > maxTokens)
                {
                    // Se uma frase sozinha for muito longa, adiciona direto
                    if (currentChunk.Count > 0)
                    {
                        chunks.Add(string.Join(" ", currentChunk).Trim());
                        currentChunk.Clear();
                        tokenCount = 0;
                    }

                    chunks.Add(sentence.Trim());
                    continue;
                }

                if (tokenCount + sentenceTokens > maxTokens)
                {
                    // Finaliza o chunk atual
                    chunks.Add(string.Join(" ", currentChunk).Trim());

                    // Prepara overlap: pega últimas frases até somar o overlapTokens
                    var overlap = new List<string>();
                    int overlapTokenCount = 0;

                    for (int j = currentChunk.Count - 1; j >= 0; j--)
                    {
                        var sent = currentChunk[j];
                        int tokens = sent.CountTokens();
                        if (overlapTokenCount + tokens > overlapTokens)
                            break;

                        overlap.Insert(0, sent);
                        overlapTokenCount += tokens;
                    }

                    currentChunk = new List<string>(overlap);
                    tokenCount = overlapTokenCount;
                }

                currentChunk.Add(sentence);
                tokenCount += sentenceTokens;
            }

            if (currentChunk.Count > 0)
            {
                chunks.Add(string.Join(" ", currentChunk).Trim());
            }

            return chunks;
        }


        private static int CountTokens(this string text)
        {
            var wordCount = Regex.Matches(text, @"\b\w+\b").Count;
            return (int)(wordCount / 0.75f); // Estimativa: 0.75 palavra ≈ 1 token
        }
    }
}
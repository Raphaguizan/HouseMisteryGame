using Guizan.LLM.Agent;
using Guizan.LLM.Agent.Actions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Guizan.LLM
{
    public class TalkResponseLLM : ResponseLLM
    {
        private List<string> pages;
        private LLMAction action;
        private List<AgentEmoticons> emoticons;

        public override string FullResponse => responseText;
        public LLMAction Action => action;
        public List<string> Pages => pages;
        public List<AgentEmoticons> Emoticons => emoticons;

        public TalkResponseLLM(string responseJson, ResponseType type = ResponseType.Success)
        {
            this.type = type;
            responseText = responseJson;
            LLMResponseTalkJSONObj response = new();
            try
            {
                
                response = JsonConvert.DeserializeObject<LLMResponseTalkJSONObj>(FindJsonInMessage(responseJson));

                pages = response.Pages;
                action = response.Action;
                emoticons = response.Emoticons;
            }
            catch (Exception e)
            {
                Debug.LogError($"LLM Respondeu um Json Inválido:\n{responseJson}");
                Debug.LogException(e);
                pages = new();
                action = new();
                this.type = ResponseType.Error;
            }
        }

        private string FindJsonInMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            try
            {
                // Padrões que lidam com chaves/colchetes aninhados usando grupos de balanceamento do .NET
                const string objectPattern = @"(?s)\{(?:(?>[^{}]+)|(?<Open>\{)|(?<-Open>\}))*\}(?(Open)(?!))";
                const string arrayPattern = @"(?s)\[(?:(?>[^\[\]]+)|(?<Open>\[)|(?<-Open>\]))*\](?(Open)(?!))";

                var objMatch = Regex.Match(message, objectPattern, RegexOptions.Singleline);
                if (objMatch.Success)
                {
                    return objMatch.Value.Trim();
                }

                var arrMatch = Regex.Match(message, arrayPattern, RegexOptions.Singleline);
                if (arrMatch.Success)
                {
                    return arrMatch.Value.Trim();
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro na regex, logamos e retornamos a mensagem original
                Debug.LogError("Erro ao tentar extrair JSON da mensagem.");
                Debug.LogException(ex);
            }

            // Se não encontrar JSON válido, retorna a própria mensagem (trimmed)
            return message.Trim();
        }

        public TalkResponseLLM(ResponseLLM response) : this(response.FullResponse, response.type){}

        public string GetFullText()
        {
            string text = "";
            for (int i = 0; i < pages.Count; i++)
            {
                text += pages[i] + "\n";
            }
            return text;
        }
        public AgentEmoticons GetEmoticon(int page)
        {
            return emoticons[page];
        }
    }
}
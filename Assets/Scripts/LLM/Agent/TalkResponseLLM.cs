using Guizan.LLM.Agent;
using Guizan.LLM.Agent.Actions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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
                response = JsonConvert.DeserializeObject<LLMResponseTalkJSONObj>(responseJson);

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
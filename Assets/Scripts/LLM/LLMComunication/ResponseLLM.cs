using Guizan.LLM.Agent.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM
{
    public class ResponseLLM
    {
        public ResponseType type;
        private string responseText;

        private List<string> pages;
        private LLMAction action;

        public string FullResponse => responseText;
        public LLMAction Action => action;
        public List<string> Pages => pages;

        public override string ToString()
        {
            return "Type: "+type+"\nText: "+responseText;
        }

        public void SetResponseData(string responseJson, ResponseType type = ResponseType.Success)
        {
            this.type = type;
            responseText = responseJson;
            LLMResponseAction response = new();
            try
            {
                response = JsonConvert.DeserializeObject<LLMResponseAction>(responseJson);
            }
            catch(Exception e)
            {
                Debug.LogError($"LLM Respondeu um Json Inválido:\n{responseJson}");
                Debug.LogException(e);
                pages = new();
                action = new();
                type = ResponseType.Error;
                return;
            }

            pages = response.Pages;
            action = response.Action;
        }
        public string GetFullText()
        {
            string text = "";
            for (int i = 0; i < pages.Count; i++)
            {
                text += pages[i] + "\n";
            }
            return text;
        }
    }

    public enum ResponseType
    {
        Success,
        Error
    }
}
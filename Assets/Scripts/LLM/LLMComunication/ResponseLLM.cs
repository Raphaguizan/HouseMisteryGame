using Guizan.LLM.Agent.Actions;
using Newtonsoft.Json;
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
            LLMResponseAction response = JsonConvert.DeserializeObject<LLMResponseAction>(responseJson);
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
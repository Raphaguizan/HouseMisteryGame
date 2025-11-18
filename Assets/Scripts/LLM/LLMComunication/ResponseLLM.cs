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
        protected string responseText;

        public virtual string FullResponse => responseText;

        public override string ToString()
        {
            return "Type: "+type+"\nText: "+responseText;
        }

        public ResponseLLM()
        {
            this.type = ResponseType.Error;
            responseText = null;
        }
        public ResponseLLM(string responseJson, ResponseType type = ResponseType.Success)
        {
            this.type = type;
            responseText = responseJson;
        }
    }

    public enum ResponseType
    {
        Success,
        Error
    }
}
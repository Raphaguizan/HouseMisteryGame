using Guizan.LLM.Agent;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent.Actions
{
    /// <summary>
    /// Representa a resposta estruturada retornada pela LLM.
    /// Contém páginas de diálogo e uma ação opcional a ser executada.
    /// </summary>
    [System.Serializable]
    public class LLMResponseAction
    {
        /// <summary>
        /// Lista de páginas de diálogo (máx. 150 caracteres cada).
        /// </summary>
        [JsonProperty("pages")]
        public List<string> Pages { get; set; }

        /// <summary>
        /// Ação opcional a ser executada (seguir, dar item, etc.).
        /// </summary>
        [JsonProperty("action")]
        public LLMAction Action { get; set; }

        public LLMResponseAction()
        {
            Pages = new List<string>();
            Action = new LLMAction();
        }
    }
}
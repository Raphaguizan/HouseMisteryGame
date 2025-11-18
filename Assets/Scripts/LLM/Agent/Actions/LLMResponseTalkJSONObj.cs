using Newtonsoft.Json;
using System.Collections.Generic;
using Guizan.LLM.Agent.Actions;

namespace Guizan.LLM.Agent
{
    /// <summary>
    /// Representa a resposta estruturada retornada pela LLM.
    /// Contém páginas de diálogo e uma ação opcional a ser executada.
    /// </summary>
    [System.Serializable]
    public class LLMResponseTalkJSONObj
    {
        /// <summary>
        /// Lista de páginas de diálogo (máx. 150 caracteres cada).
        /// </summary>
        [JsonProperty("pages")]
        public List<string> Pages { get; set; }
        
        /// <summary>
        /// Tipo de emoção que a arte do personagem deve expressar em cada página
        /// </summary>
        [JsonProperty("Emoticon")]
        public List<AgentEmoticons> Emoticons { get; set; }

        /// <summary>
        /// Ação opcional a ser executada (seguir, dar item, etc.).
        /// </summary>
        [JsonProperty("action")]
        public LLMAction Action { get; set; }

        public LLMResponseTalkJSONObj()
        {
            Pages = new List<string>();
            Action = new LLMAction();
        }
    }
}
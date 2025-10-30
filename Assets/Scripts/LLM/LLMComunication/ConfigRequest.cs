using System;
using System.Collections.Generic;

namespace Guizan.LLM
{
    [Serializable]
    public class ConfigRequest
    {
        public List<Message> messages;
        public string model;
        public float temperature;
        public int max_completion_tokens;
        public float top_p;
        public bool stream;
        public string stop;


        public ConfigRequest(List<Message> messages, string model = "gemma2-9b-it", float temperature = 1f, int maxTokens = 1024, float topP = 1f, bool stream = false, string stop = null)
        {
            this.messages = messages;
            this.model = model;
            this.temperature = temperature;
            this.max_completion_tokens = maxTokens;
            this.top_p = topP;
            this.stream = stream;
            this.stop = stop;
        }
    }
}
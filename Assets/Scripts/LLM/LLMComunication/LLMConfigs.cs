using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM
{
    [CreateAssetMenu(fileName = "NewAgentConfig", menuName = "LLM/Config")]
    public class LLMConfigs : ScriptableObject
    {
        private List<Message> messages;
        [SerializeField]
        private string model = "gemma2-9b-it";
        [Range(0f, 2f)]
        [SerializeField]
        private float temperature = 1f;
        [SerializeField]
        private int max_completion_tokens = 1024;
        [SerializeField]
        private float top_p = 1f;
        [SerializeField]
        private bool stream = false;
        [SerializeField]
        private string stop = null;

        public ConfigRequest GetRequest()
        {
            return new ConfigRequest(messages, model, temperature, Max_completion_tokens, Top_p, Stream, Stop);
        }

        public string Model => model;
        public float Temperature => temperature;
        public int Max_completion_tokens => max_completion_tokens;
        public float Top_p => top_p;
        public bool Stream => stream;
        public string Stop => stop;
    }
}
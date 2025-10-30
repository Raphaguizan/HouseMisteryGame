using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Guizan.LLM
{
    [CreateAssetMenu(fileName = "NewAgentConfig", menuName = "LLM/AgentConfig")]
    public class AgentConfigs : ScriptableObject
    {
        [SerializeField]
        private string agentID;

        [SerializeField]
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


        [Space]
        public UnityEvent<string> MessagesChangeEvent;

        public string AgentID => agentID;
        public List<Message> Messages => messages;
        public int Lenght => Messages.Count;

        private void Awake()
        {
            MessagesChangeEvent = new();
        }

        public void LoadMessages(List<Message> messages)
        {
            this.messages = messages;
            this.MessagesChangeEvent.Invoke(messages[Lenght - 1].role);
        }

        public void ResetMessages()
        {
            this.messages = new()
            {
                new(MessageRole.system, "Você está interpretando um personagem de RPG. Suas respostas devem refletir a personalidade, emoções, experiências e conhecimentos do personagem que você representa. Essas características podem ser fornecidas inicialmente ou atualizadas ao longo da conversa por meio de informações de contexto (embeddings) ou instruções específicas.\r\n\r\nSempre responda como se fosse o personagem, utilizando o estilo de fala, vocabulário e atitude apropriados. Seja coerente com o estado emocional atual, a memória do personagem e sua história.\r\n\r\n- Não use emojis ou emoticons.\r\n- Não mencione que você é uma IA ou um assistente.\r\n- Não forneça informações genéricas ou fora do papel.\r\n- Mantenha as respostas imersivas, como se estivesse vivendo naquele mundo.\r\n\r\nAs características do personagem (personalidade, memórias, histórico, humor atual, etc.) serão fornecidas diretamente ou indiretamente pelo sistema. Adapte-se a essas informações conforme necessário.\r\n")
            };
            this.MessagesChangeEvent.Invoke("system");
        }

        public void AddMessage(Message message)
        {
            this.messages.Add(message);
            this.MessagesChangeEvent.Invoke(message.role);
        }

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

        private void Reset()
        {
            ResetMessages();
        }
    }
}
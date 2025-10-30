using NaughtyAttributes;
using Newtonsoft.Json;
using System;

namespace Guizan.LLM
{
    public enum MessageRole
    {
        user,
        system,
        assistant
    }

    [Serializable]
    public class Message 
    {
        [JsonProperty("role"), Dropdown("rolesNames")]
        public string role;
        [JsonProperty("content"), ResizableTextArea]
        public string content;

        private static string[] rolesNames => Enum.GetNames(typeof(MessageRole));
        public Message(MessageRole role, string content)
        {
            this.role = role.ToString();
            this.content = content;
        }

        public bool CompareRole(MessageRole roleToCompare)
        {
            return role.Equals(roleToCompare.ToString());
        }
    }
}

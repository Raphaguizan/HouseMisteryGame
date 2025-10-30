using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM
{
    [CreateAssetMenu(fileName = "newPermanentMemory", menuName = "LLM/Permanent memory")]
    public class PermanentMemory : ScriptableObject
    {
        [SerializeField]
        private Message my_message;

        public Message Message => my_message;
    }
}
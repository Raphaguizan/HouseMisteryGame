using System.Runtime.Serialization;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    public interface ITalkInjector
    {
        public string GetTextToInject();
    }
}
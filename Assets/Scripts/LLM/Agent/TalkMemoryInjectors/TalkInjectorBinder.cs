using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    public class TalkInjectorBinder : ScriptableObject, ITalkInjector
    {
        public virtual string GetTextToInject()
        {
            return "";
        }
    }
}
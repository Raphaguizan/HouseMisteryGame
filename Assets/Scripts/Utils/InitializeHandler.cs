using Game.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Initialization
{
    public class InitializeHandler : Singleton<InitializeHandler>
    {
        [SerializeField, Tooltip("Time out in seconds to Initialize")]
        private float timeOut = 5f;

        private static Dictionary<string, bool> Initialized = new();

        override protected void Awake()
        {
            base.Awake();
            Initialized = new Dictionary<string, bool>();
        }
        public static void SubscribeInitialization(string key)
        {
            if (!Initialized.ContainsKey(key))
            {
                Initialized.Add(key, false);
                Instance.StartCoroutine(Instance.TimeOutCounter(key));
            }
        }

        private IEnumerator TimeOutCounter(string keyToCount)
        {
            yield return new WaitForSeconds(timeOut);
            if (!IsInitialized(keyToCount))
            {
                Debug.LogWarning($"Initialization Timeout for {keyToCount}");
                SetInitialized(keyToCount);
            }
        }

        public static void SetInitialized(string key)
        {
            if (Initialized.ContainsKey(key))
            {
                Initialized[key] = true;
            }
        }

        public static bool IsInitialized(string key)
        {
            if (Initialized.ContainsKey(key))
            {
                return Initialized[key];
            }
            return true;
        }

        public static bool IsAllInitialized()
        {
            foreach (var item in Initialized)
            {
                if (!item.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
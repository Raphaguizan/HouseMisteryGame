using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizan.Dialog;

namespace Guizan.NPC
{
    public class NPCController : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private NPCConfigs _myConfigs;

        [SerializeField]
        private SpriteRenderer mockOver;

        private Color origColor;

        public void Interact()
        {
            DialogManager.InitializeDialog(_myConfigs);
            Debug.Log("Interagiu");
        }

        public void OnPointerOver(bool val)
        {
            if(val)
                mockOver.color = Color.red;
            else
                mockOver.color = origColor;
        }

        void Start()
        {
            origColor = mockOver.color;
        }       
    }
}
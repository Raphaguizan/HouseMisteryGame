using Guizan.LLM.Agent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guizan.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputsController : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovement movement;

        [SerializeField]
        private PlayerHands hands;

        private AgentTalkManager talkManager;
        private void Awake()
        {
            talkManager = FindAnyObjectByType<AgentTalkManager>();
        }

        private bool VerifyCanWalk()
        {
            if (talkManager == null)
                return true;

            return !talkManager.InConversation;
        }

        public void OnMove(InputValue action)
        {
            if (!VerifyCanWalk())
                return;

            Vector2 vec = action.Get<Vector2>();
            movement.Move(vec);
        }

        public void OnJump(InputValue action)
        {
            if (!VerifyCanWalk())
                return;

            bool val = action.isPressed;
            movement.Jump(val);
        }

        public void OnInteract()
        {
            if (!VerifyCanWalk())
                return;

            hands.Interact();
        }
    }
}
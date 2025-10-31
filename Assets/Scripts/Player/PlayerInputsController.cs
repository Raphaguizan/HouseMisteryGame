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

        private void OnMove(InputValue action)
        {
            Vector2 vec = action.Get<Vector2>();
            movement.Move(vec);
        }

        private void OnJump(InputValue action)
        {
            bool val = action.isPressed;
            movement.Jump(val);
        }

        private void OnInteract(InputValue action)
        {
            hands.Interact();
        }
    }
}
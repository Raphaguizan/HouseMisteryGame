using DG.Tweening;
using NaughtyAttributes.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private PlayerFeet feet;

        [Header("Parameters")]
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        private float jumpForce = 3f;
        [SerializeField]
        private float downForce = 1f;
        [SerializeField]
        private float accelerationFactor = .1f;
        

        private float xVelocity = 0f;
        private Coroutine currentMoveCoroutine = null;

        public void OnMove(InputValue val)
        {
            Vector2 inputVal = val.Get<Vector2>();
            if (currentMoveCoroutine != null)
                StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = StartCoroutine(SpeedXFlow(inputVal.x));
        }

        private IEnumerator SpeedXFlow(float to)
        {
            float factor = accelerationFactor;
            if (xVelocity > to)
                factor *= -1;

            //Debug.Log($"xvelocity : {xVelocity}, to: {to}, factor: {factor}");
            while (factor < 0? xVelocity > to : xVelocity < to)
            {
                yield return new WaitForSeconds(.001f);
                xVelocity += factor/10;
            }
            xVelocity = to;
        }

        public void OnJump(InputValue val)
        {
            if (val.isPressed && feet.FeetOnFloor)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        private void FixedUpdate()
        {
            rb.AddForce(downForce * 100f * Time.fixedDeltaTime * Vector2.down);
            rb.velocity = new(speed * 100f * xVelocity * Time.fixedDeltaTime, rb.velocity.y);
        }
    }
}
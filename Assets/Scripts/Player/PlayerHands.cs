using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.Player
{
    public class PlayerHands : MonoBehaviour
    {
        private IInteractable myInteractable;
        private GameObject interactableGO;
        private PlayerMovement myPlayerMovement;
        private Rigidbody2D myRB;
        private void Awake()
        {
            myPlayerMovement = GetComponentInParent<PlayerMovement>();
            if(myPlayerMovement != null)
                myRB = myPlayerMovement.GetComponent<Rigidbody2D>();
        }

        public void Interact()
        {
            if (myInteractable == null)
                return;
            myInteractable.Interact();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            IInteractable colliderInteract = collision.GetComponent<IInteractable>();
            if (colliderInteract != null && myInteractable != null && !myInteractable.Equals(colliderInteract))
                IncludeCollision();

            IncludeCollision(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            IInteractable colliderInteract = collision.GetComponent<IInteractable>();
            if (colliderInteract != null && colliderInteract.Equals(myInteractable))           
                IncludeCollision();            
        }

        private void IncludeCollision(Collider2D collision = null)
        {
            if (collision != null)
            {
                interactableGO = collision.gameObject;
                myInteractable = collision.GetComponent<IInteractable>();
                myInteractable?.OnPointerOver(true);
            }
            else
            {
                myInteractable?.OnPointerOver(false);
                myInteractable = null;
                interactableGO = null;
            }
        }

        private void FixedUpdate()
        {
            if ((myRB.velocity.x > 0 && transform.localPosition.x < 0) || (myRB.velocity.x < 0 && transform.localPosition.x > 0))
                transform.localPosition = transform.localPosition * new Vector2(-1, 1);
        }
    }
}
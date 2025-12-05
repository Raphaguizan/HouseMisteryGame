using Guizan.NPC;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RoomPlayerCollider : MonoBehaviour
    {
        [SerializeField]
        private GameObject vCamera;
        [SerializeField, Tag]
        private string playerTag = "Player";

        private PolygonCollider2D my_poligon;

        public PolygonCollider2D Poligon => my_poligon;

        public void SetColliderPoints(Vector2[] newPoints)
        {
            my_poligon.SetPath(0, newPoints);
        }
        private void Awake()
        {
            my_poligon = GetComponent<PolygonCollider2D>();
        }        

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.CompareTag(playerTag)) return;

            vCamera.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag(playerTag)) return; 
            
            vCamera.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            NPCController npc = collision.GetComponent<NPCController>();
            if (npc == null) return;

            npc.SetCameraBounds(this.gameObject);
        }
    }
}
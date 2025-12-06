using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizan.Dialog;
using Unity.VisualScripting;
using Guizan.House.Room;
using System.Linq;
using NaughtyAttributes;
using System;

namespace Guizan.NPC
{
    public class NPCController : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private NPCConfigs _myConfigs;

        [SerializeField]
        private SpriteRenderer mockOver;

        [SerializeField]
        private TMPro.TextMeshProUGUI mockName;

        private GameObject currentCameraBounds;

        private Color origColor;

        public Action RoomChanged;

        public void Interact()
        {
            DialogManager.InitializeDialog(_myConfigs);
            //Debug.Log("Interagiu");
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
            mockName.text = _myConfigs.name;
            _myConfigs.SetNPCPrefab(this.gameObject);
        }
        
        public RoomController GetCurrentRoom()
        {
            if( currentCameraBounds == null)
                return null;
            return currentCameraBounds.GetComponentInParent<RoomController>();
        }

        public void SetCameraBounds(GameObject currentCameraBounds)
        {
            this.currentCameraBounds = currentCameraBounds;
            RoomChanged?.Invoke();
        }

        public (Vector2, float) GetRoomBounds()
        {
            if (currentCameraBounds == null)
                return (Vector2.negativeInfinity, -1f);
            PolygonCollider2D collider = currentCameraBounds.GetComponent<PolygonCollider2D>();
            if (collider == null)
                return (Vector2.negativeInfinity, -1f);

            Vector2 min = collider.points.OrderBy(v => v.x).FirstOrDefault();
            Vector2 max = collider.points.OrderBy(v => v.x).LastOrDefault();

            Vector2 worldMin = transform.localToWorldMatrix.MultiplyPoint(min);
            Vector2 worldMax = transform.localToWorldMatrix.MultiplyPoint(max);

            return (new(worldMin.x, worldMax.x), collider.bounds.center.y);
        }
    }
}
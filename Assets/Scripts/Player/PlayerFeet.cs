using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Guizan.Player
{
    public class PlayerFeet : MonoBehaviour
    {
        [SerializeField]
        private float feetDist = 3f;
        [SerializeField]
        private LayerMask ignoreLayer;

        public bool FeetOnFloor => CheckFeet();
        private bool CheckFeet()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, feetDist, ~ignoreLayer);
            //if (hit.collider != null)
            //    Debug.Log(hit.collider.name + "    is trigger: "+hit.collider.isTrigger);
            return hit.collider != null && !hit.collider.isTrigger;
        }
    }
}
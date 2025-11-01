using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return hit.collider != null && !hit.collider.isTrigger;
        }
    }
}
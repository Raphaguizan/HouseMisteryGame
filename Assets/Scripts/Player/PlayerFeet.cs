using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.Player
{
    public class PlayerFeet : MonoBehaviour
    {
        [SerializeField]
        private float feetDist = 3f;

        public bool FeetOnFloor => CheckFeet();
        private bool CheckFeet()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, feetDist);
            return hit.collider != null;
        }
    }
}
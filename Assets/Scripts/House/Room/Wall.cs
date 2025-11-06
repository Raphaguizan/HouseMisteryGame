using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    public class Wall : MonoBehaviour
    {
        [SerializeField]
        private WallType type;

        public WallType Type => type;
    }
}
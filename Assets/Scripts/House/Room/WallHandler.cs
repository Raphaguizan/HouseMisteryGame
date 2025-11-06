using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    public class WallHandler : MonoBehaviour
    {
        [SerializeField]
        private List<Wall> myWalls;

        private WallType currentType = WallType.Full;
        public WallType WallType => currentType;

        private void Start()
        {
            ConfigureWall(currentType);
        }

        public void ConfigureWall(WallType type)
        {
            currentType = type;
            for (int i = 0; i < myWalls.Count; i++)
            {
                myWalls[i].gameObject.SetActive(myWalls[i].Type == type);
            }
        }
    }
}
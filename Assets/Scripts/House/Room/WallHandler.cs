using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    public class WallHandler : MonoBehaviour
    {
        [SerializeField]
        private List<Wall> myWalls;

        private void Start()
        {
            for (int i = 0; i < myWalls.Count; i++)
            {
                myWalls[i].gameObject.SetActive(false);
            }
        }

        public void ConfigureWall(WallType type)
        {
            for (int i = 0; i < myWalls.Count; i++)
            {
                myWalls[i].gameObject.SetActive(myWalls[i].Type == type);
            }
        }
    }
}
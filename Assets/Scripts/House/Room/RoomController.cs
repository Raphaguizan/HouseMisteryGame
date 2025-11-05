using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    public enum WallType
    {
        None,
        Full,
        Door
    }
    public class RoomController : MonoBehaviour
    {
        [SerializeField]
        private RoomPlayerCollider roomCollider;

        [Header("Handlers")]
        [SerializeField]
        private WallHandler leftWallHandler;
        [SerializeField]
        private WallHandler rightWallHandler;
        [SerializeField]
        private WallHandler floorHandler;
        [SerializeField]
        private WallHandler ceilingHandler;

        public void ConfigureRoom(WallType left = WallType.Full, WallType right = WallType.Full, WallType floor = WallType.Full, WallType ceiling = WallType.Full)
        {
            leftWallHandler.ConfigureWall(left);
            rightWallHandler.ConfigureWall(right);
            floorHandler.ConfigureWall(floor);
            ceilingHandler.ConfigureWall(ceiling);
        }
    }
}
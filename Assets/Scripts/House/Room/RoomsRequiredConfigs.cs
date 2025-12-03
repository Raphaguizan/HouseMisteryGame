using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Guizan.House.Room
{
    [Serializable]
    public class RequiredConfig
    {
        public RoomType MyType = RoomType.Default;
        public int NumDoors = 1;
        public bool CanHaveStairs = true;
    }
    [CreateAssetMenu(fileName = "NewRoomRequiredConfig", menuName ="Rooms/RequiredConfig")]
    public class RoomsRequiredConfigs : ScriptableObject
    {
        [SerializeField]
        private List<RequiredConfig> configs;

        public RequiredConfig this[int index] => configs[index];
        public int Count => configs.Count;

        public int CountDoors(int i) => configs.Count(c => c.NumDoors == i);

        public int MaxDoorsCount => configs.Max(c => c.NumDoors);
    }
}
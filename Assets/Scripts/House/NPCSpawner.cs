using Game.Initialization;
using Guizan.House.Room;
using Guizan.NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House
{
    [RequireComponent(typeof(RoomsTypeHandler))]
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<NPCSpawnRoom> npcSpawnRooms;
        private RoomsTypeHandler roomsTypeHandler;

        private List<NPCController> NPCsSpawneds = new();

        private void Awake()
        {
            InitializeHandler.SubscribeInitialization(this.GetType().Name);
            roomsTypeHandler = GetComponent<RoomsTypeHandler>();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => InitializeHandler.IsInitialized(typeof(NavMeshHandler).Name));
            SpawnNPCs();
            InitializeHandler.SetInitialized(this.GetType().Name);
        }

        private void SpawnNPCs()
        {
            var myRooms = roomsTypeHandler.manager.GetRooms();
            for (int i = 0; i < npcSpawnRooms.Count; i++)
            {
                for (int j = 0; j < myRooms.Count; j++)
                {
                    if (myRooms[j].RoomType == npcSpawnRooms[i].roomType)
                    {
                        var newNPC = Instantiate(npcSpawnRooms[i].NPCPrefab, myRooms[j].transform.position, Quaternion.identity);
                        NPCsSpawneds.Add(newNPC.GetComponent<NPCController>());
                        break;
                    }
                }
            }
        }
    }

    [Serializable]
    public class NPCSpawnRoom
    {
        public GameObject NPCPrefab;
        public RoomType roomType;
    }
}
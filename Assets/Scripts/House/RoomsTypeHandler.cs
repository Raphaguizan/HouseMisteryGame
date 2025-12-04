using Game.Initialization;
using Guizan.House.Room;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Guizan.House
{
    [RequireComponent(typeof(HouseManager))]
    public class RoomsTypeHandler : MonoBehaviour
    {
        [SerializeField, Expandable]
        private RoomsRequiredConfigs _roomsRequiredConfig;
        [SerializeField]
        private List<RoomController> _rooms;
        [SerializeField]
        private List<RoomController> rooms1Door = new();
        [SerializeField]
        private List<RoomController> rooms2Door = new();
        [SerializeField]
        private List<RoomController> roomsHasStair = new();

        [HideInInspector]
        public HouseManager manager;
        private void Awake()
        {
            InitializeHandler.SubscribeInitialization(this.GetType().Name);
            manager = GetComponent<HouseManager>();
            _rooms = new();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => InitializeHandler.IsInitialized(manager.GetType().Name));
            _rooms = manager.GetRooms();
            ClassifyRooms(_rooms);
            SetRooms();
            manager.AdjustCamerasBounds();
            InitializeHandler.SetInitialized(this.GetType().Name);
        }

        public void ClassifyRooms(List<RoomController> rooms)
        {
            foreach (RoomController room in rooms)
            {
                if (room.CurerntPos == Vector2.zero)
                    continue;
                int doorCount = room.CountDoors();
                if (doorCount == 1 && room.GetWallType(WallSide.Ceiling) != WallType.Door && room.GetWallType(WallSide.Ceiling) != WallType.Door2)
                    rooms1Door.Add(room);
                else if(HasStairs(room))
                    roomsHasStair.Add(room);           
                else if (doorCount == 2)
                    rooms2Door.Add(room);
            }
        }

        // TODO selecionar salas com base em critérios específicos

        public void SelectRoom(RoomController selectedRoom, RoomType selectedType)
        {
            selectedRoom.SetRoomType(selectedType);
            // muda o tipo de parede para porta 1 tanto na sala selecionada quanto na sala vizinha
            foreach (WallSide side in selectedRoom.GetDoorsSide())
            {
                selectedRoom.ChangeWallType(side, WallType.Door);
                manager.GetNeighborRoom(selectedRoom, side)?.ChangeWallType(selectedRoom.OpositeWallSide(side), WallType.Door);
            }
        }

        private void SetRooms()
        {
            for (int i = 0; i < _roomsRequiredConfig.Count; i++)
            {
                var selectedList = GetListByNumDoors(_roomsRequiredConfig[i].NumDoors);
                if (selectedList.Count == 0)
                {
                    var newRoom = CreateNewRoom(_roomsRequiredConfig[i].NumDoors, _roomsRequiredConfig[i].CanHaveStairs);
                    selectedList.Add(newRoom);
                }
                int randomPos = Random.Range(0, selectedList.Count);
                SelectRoom(selectedList[randomPos], _roomsRequiredConfig[i].MyType);
                selectedList.RemoveAt(randomPos);
            }
        }
        private RoomController CreateNewRoom(int doorsNum = 1, bool canHaveStair = false)
        {
            Vector2Int roomPos = new();
            bool RoomFound = false;
            List<WallSide> doorsSideToOpen = new();
            while (!RoomFound)
            {
                roomPos = manager.GetRandomEmptySlot();
                var neighbours = manager.FindRoomNeigbours(roomPos);
                
                neighbours.Remove(WallSide.Ceiling);
                if (!canHaveStair)
                    neighbours.Remove(WallSide.Floor);
                
                if (neighbours.Count >= doorsNum)
                {
                    for(int i = 0; i < doorsNum; i++)
                    {
                        int randomIndex = Random.Range(0, neighbours.Count);
                        doorsSideToOpen.Add(neighbours[randomIndex]);
                        neighbours.RemoveAt(randomIndex);
                    }
                    RoomFound = true;
                }
            }
            RoomController newRoom = manager.CreateRoom(roomPos);
            for (int i = 0; i < doorsSideToOpen.Count; i++)
            {
                var currentNeighbour = manager.GetNeighborRoom(newRoom, doorsSideToOpen[i]);
                manager.OpenPassage(newRoom.CurerntPos, currentNeighbour.CurerntPos);
            }
            Debug.Log($"Criada nova sala em {roomPos} com {doorsNum} portas.");
            _rooms.Add(newRoom);
            return newRoom;
        }
        private bool CheckRooms()
        {

            for (int i = 1; i < _roomsRequiredConfig.MaxDoorsCount; i++)
            {
                var selectedList = GetListByNumDoors(i);
                if (_roomsRequiredConfig.CountDoors(i) > selectedList.Count)
                {
                    Debug.LogWarning($"Não há quartos suficientes com {i} portas para atender à configuração necessária.");
                    return false;
                }
            }
            return true;
        }

        private bool HasStairs(RoomController room)
        {
            bool door1 = room.GetWallType(WallSide.Floor) == WallType.Door || room.GetWallType(WallSide.Ceiling) == WallType.Door;
            bool door2 = room.GetWallType(WallSide.Floor) == WallType.Door2 || room.GetWallType(WallSide.Ceiling) == WallType.Door2;
            return door1 || door2;
        }
        private List<RoomController> GetListByNumDoors(int i)
        {
            // adicionar nesse Switch todas as listas de comodos classificados
            return i switch
            {
                1 => rooms1Door,
                2 => rooms2Door,
                _ => roomsHasStair
            };
        }
    }
}
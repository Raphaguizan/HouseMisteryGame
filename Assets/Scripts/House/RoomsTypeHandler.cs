using Guizan.House.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House
{
    [RequireComponent(typeof(HouseManager))]
    public class RoomsTypeHandler : MonoBehaviour
    {
        [SerializeField]
        private List<RoomController> rooms;
        private HouseManager manager;
        private void Awake()
        {
            manager = GetComponent<HouseManager>();
            rooms = new();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => manager.IsInitialized);
            rooms = manager.GetRooms();
        }

        //private void ChooseSecretRoom()
        //{
        //    List<Vector2Int> oneDoorRoons = new();
        //    List<Vector2Int> oneDoorRoonsHorizontal = new();

        //    for (int x = 0;x < matrixDim.x; x++)
        //        for(int y = 0;y < matrixDim.y; y++)
        //            if (roonsMatrix[x, y] != null && roonsMatrix[x, y].CountDoors() == 1 && !(x == 0 && y == 0))
        //                oneDoorRoons.Add(new(x, y));

        //    for (int i = 0; i < oneDoorRoons.Count; i++)
        //    {
        //        RoomController myRoom = roonsMatrix[oneDoorRoons[i].x, oneDoorRoons[i].y];
        //        WallSide doorSide = myRoom.GetDoorsSide()[0];
        //        if (doorSide == WallSide.Left || doorSide == WallSide.Right)
        //            oneDoorRoonsHorizontal.Add(oneDoorRoons[i]);
        //    }

        //    if(oneDoorRoonsHorizontal.Count > 0)
        //    {
        //        secretRoom = oneDoorRoonsHorizontal[rand.Next(oneDoorRoonsHorizontal.Count)];
        //        RoomController myRoom = roonsMatrix[secretRoom.Value.x, secretRoom.Value.y];
        //        myRoom.name += " S";

        //        WallSide doorSide = myRoom.GetDoorsSide()[0];
        //        myRoom.ChangeWallType(doorSide, WallType.Door);

        //        //Debug.Log($"ChoosenRoom = ({secretRoom.Value.x},{secretRoom.Value.y})");
        //    }

        //    if (!secretRoom.HasValue)
        //        Debug.LogWarning("Não foi gerado um quarto secreto!");
        //}
    }
}
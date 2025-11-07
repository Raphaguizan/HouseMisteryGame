using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Guizan.House.Room
{
    public enum WallType
    {
        Door,
        Door2,
        Full
    }

    public enum WallSide
    {
        Left,
        Right,
        Floor,
        Ceiling
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

        public Vector2[] ColPoligonPoints => TransformPointsToWorldPos(roomCollider.Poligon.points);
        public bool HasCollider => roomCollider.gameObject.activeInHierarchy;

        private Vector2[] TransformPointsToWorldPos(Vector2[] points)
        {
            Vector2[] resp = new Vector2[points.Length];

            for (int i = 0; i < points.Length; i++) 
            {
                resp[i] = transform.TransformPoint(points[i]);
            }
            return resp;
        }

        private Vector2[] TransformWorldPosToPoints(Vector2[] points)
        {
            Vector2[] resp = new Vector2[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                resp[i] = transform.InverseTransformPoint(points[i]);
            }
            return resp;
        }

        public void AdaptColliderPointsToRight(RoomController rightOne)
        {
            var pointsIndexToChange = SideIndex(WallSide.Right);

            Vector2[] result = new Vector2[ColPoligonPoints.Length];
            for (int i = 0; i < ColPoligonPoints.Length; i++)
            {
                if (pointsIndexToChange.Contains(i))
                    result[i] = rightOne.ColPoligonPoints[i];
                else
                    result[i] = ColPoligonPoints[i];
            }

            roomCollider.SetColliderPoints(TransformWorldPosToPoints(result));
            rightOne.SetColliderActive(false);
        }

       public int[] SideIndex(WallSide side)
        {
            return side switch
            {
                WallSide.Left => new int[2] { 2, 3 },
                WallSide.Right => new int[2] { 0, 1 },
                WallSide.Floor => new int[2] { 0, 3 },
                _ => new int[2] { 1, 2 }
            };
        }

        public void SetColliderActive(bool val = true)
        {
            roomCollider.gameObject.SetActive(val);
        }

        public void ConfigureRoom(WallType left = WallType.Full, WallType right = WallType.Full, WallType floor = WallType.Full, WallType ceiling = WallType.Full)
        {
            leftWallHandler.ConfigureWall(left);
            rightWallHandler.ConfigureWall(right);
            floorHandler.ConfigureWall(floor);
            ceilingHandler.ConfigureWall(ceiling);
        }

        public void ChangeWallType(WallSide side, WallType type)
        {
            GetHandler(side).ConfigureWall(type);
        }

        public WallType GetWallType(WallSide side)
        {
            return GetHandler(side).WallType;
        }

        public int CountDoors(Nullable<WallType> type = null)
        {
            int resp = 0;
            if (type == null)
            {
                resp += GetDoorsSide(WallType.Door).Count;
                resp += GetDoorsSide(WallType.Door2).Count;
                return resp;
            }

            return GetDoorsSide(type.Value).Count;
        }

        public List<WallSide> GetDoorsSide()
        {
            List<WallSide>  resp = GetDoorsSide(WallType.Door);
            resp.AddRange(GetDoorsSide(WallType.Door2));

            return resp;
        }

        public List<WallSide> GetDoorsSide(WallType type)
        {
            List<WallSide> resp = new();

            if (leftWallHandler.WallType == type)
                resp.Add(WallSide.Left);
            if (rightWallHandler.WallType == type)
                resp.Add(WallSide.Right);
            if (floorHandler.WallType == type)
                resp.Add(WallSide.Floor);
            if (ceilingHandler.WallType == type)
                resp.Add(WallSide.Ceiling);

            return resp;
        }

        private WallHandler GetHandler(WallSide side)
        {
            WallHandler myHandler = side switch
            {
                WallSide.Left => leftWallHandler,
                WallSide.Right => rightWallHandler,
                WallSide.Ceiling => ceilingHandler,
                _ => floorHandler,
            };
            return myHandler;
        }

    }
}
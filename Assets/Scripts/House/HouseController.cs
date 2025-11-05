using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House
{
    public class HouseController : MonoBehaviour
    {
        [SerializeField]
        private GameObject roomPrefab;
        [SerializeField]
        private Transform roonsParent;
        [SerializeField]
        private Vector2Int matrixDim;
        [SerializeField]
        private float horizontalDist;
        [SerializeField]
        private float verticalDist;

        private List<Transform> roonsList;
        private Vector2[,] matrix;

        void Start()
        {
            MakeMatrix(matrixDim.x, matrixDim.y);
            InstantiateRoons();
        }

        private void InstantiateRoons()
        {
            roonsList = new();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    var newRoom = Instantiate(roomPrefab, roonsParent).transform;
                    newRoom.localPosition = matrix[i, j];
                    roonsList.Add(newRoom);
                }
            }
        }

        private void MakeMatrix(int x, int y)
        {
            matrix = new Vector2[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    matrix[i, j] = new(i*horizontalDist, j*verticalDist);
                }
            }
        }
    }
}
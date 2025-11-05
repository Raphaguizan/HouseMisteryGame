using Guizan.House.Room;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House
{
    public class HouseMatrix
    {
        private Vector2[,] matrix;

        HouseMatrix(int x, int y)
        {
            matrix = new Vector2[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    matrix[i,j] = new(i,j);
                }
            }
        }
    }
}
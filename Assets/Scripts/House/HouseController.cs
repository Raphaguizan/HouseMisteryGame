using Guizan.House.Room;
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
        [Space, SerializeField, Range(0f,1f)]
        private float ChanceToDoorOpen = .2f;
        [SerializeField, Range(0f,1f)]
        private float horizontalBias = .75f;

        private RoomController[,] roonsMatrix;
        private System.Random rand = new System.Random();

        void Start()
        {            
            InstantiateRoons();
        }

        private void InstantiateRoons()
        {
            roonsMatrix = new RoomController[matrixDim.x, matrixDim.y];

            for (int x = 0; x < matrixDim.x; x++)
            {
                for (int y = 0; y < matrixDim.y; y++)
                {
                    var newRoom = Instantiate(roomPrefab, roonsParent).transform;
                    newRoom.localPosition = new(x * horizontalDist, y * verticalDist);
                    newRoom.name = $"Room({x},{y})";
                    roonsMatrix[x, y] = newRoom.GetComponent<RoomController>();
                }
            }

            // Começa DFS de geração
            bool[,] visited = new bool[matrixDim.x, matrixDim.y];
            DFSGenerate(0, 0, visited);
            AdjustCamerasBounds();

            Debug.Log("Mapa gerado com sucesso e totalmente conectado!");
        }

        private void DFSGenerate(int x, int y, bool[,] visited)
        {
            visited[x, y] = true;

            // Direções possíveis
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),   // cima
                new Vector2Int(0, -1),  // baixo
                new Vector2Int(1, 0),   // direita
                new Vector2Int(-1, 0)   // esquerda
            };

            // Embaralha as direções
            ShuffleDirections(directions);

            foreach (var dir in directions)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;

                // Pula se for fora da matriz
                if (nx < 0 || ny < 0 || nx >= matrixDim.x || ny >= matrixDim.y)
                    continue;

                // Se ainda não visitou, abre uma passagem e continua o DFS
                if (!visited[nx, ny])
                {
                    OpenPassage(x, y, nx, ny);
                    DFSGenerate(nx, ny, visited);
                }
            }
        }

        private void OpenPassage(int x1, int y1, int x2, int y2)
        {
            RoomController current = roonsMatrix[x1, y1];
            RoomController neighbor = roonsMatrix[x2, y2];
            WallType randomType = WallType.Door;

            if (Random.value < ChanceToDoorOpen)
                randomType = WallType.Door2;
            // Determina a direção da conexão
            if (x2 > x1)
            {
                // vizinho à direita
                current.ChangeWallType(WallSide.Right, randomType);
                neighbor.ChangeWallType(WallSide.Left, randomType);
            }
            else if (x2 < x1)
            {
                // vizinho à esquerda
                current.ChangeWallType(WallSide.Left, randomType);
                neighbor.ChangeWallType(WallSide.Right, randomType);
            }
            else if (y2 > y1)
            {
                // vizinho acima
                current.ChangeWallType(WallSide.Ceiling, randomType);
                neighbor.ChangeWallType(WallSide.Floor, randomType);
            }
            else if (y2 < y1)
            {
                // vizinho abaixo
                current.ChangeWallType(WallSide.Floor, randomType);
                neighbor.ChangeWallType(WallSide.Ceiling, randomType);
            }
        }

        private void AdjustCamerasBounds()
        {
            for (int x = 0; x < matrixDim.x; x++)
            {
                for (int y = 0; y < matrixDim.y; y++)
                {
                    if (roonsMatrix[x, y].GetWallType(WallSide.Right) == WallType.Door2)
                    {
                        int xIndex = x;
                        while(!roonsMatrix[xIndex, y].HasCollider)
                        {
                            xIndex--;
                        }
                        roonsMatrix[xIndex, y].AdaptColliderPointsToRight(roonsMatrix[x+1, y]);                    
                    }
                }
            }
        }

        private void GenerateWalls()
        {
            for (int x = 0; x < matrixDim.x; x++)
            {
                for (int y = 0; y < matrixDim.y; y++)
                {
                    RoomController room = roonsMatrix[x, y];

                    // 🔹 Determina o estado de cada parede
                    WallType left = WallType.Full;
                    WallType right = WallType.Full;
                    WallType floor = WallType.Full;
                    WallType ceiling = WallType.Full;

                    // --- PAREDE ESQUERDA ---
                    if (x != 0)                    
                    {
                        // usa o mesmo estado da parede direita do cômodo anterior
                        RoomController leftNeighbor = roonsMatrix[x - 1, y];
                        left = leftNeighbor == null ? WallType.Full : leftNeighbor.GetWallType(WallSide.Right);
                    }

                    // --- PAREDE DIREITA ---
                    if (x < matrixDim.x - 1)
                    {
                        // decide se há passagem (None, Door, Full)
                        right = GenerateHorizontalWallType();
                    }

                    // --- PISO ---
                    if (y != 0)
                    {
                        RoomController below = roonsMatrix[x, y - 1];
                        floor = below == null ? WallType.Full : below.GetWallType(WallSide.Ceiling);       
                    }

                    // --- TETO ---
                    if (y < matrixDim.y - 1)
                    {
                        ceiling = GenerateVerticalWallType();
                    }

                    room.ConfigureRoom(left, right, floor, ceiling);
                }
            }
        }

        private WallType GenerateHorizontalWallType()
        {
            float r = Random.value;
            if (r < 0.2f) return WallType.Door2; // 20% sem parede (salas unidas)
            if (r < 0.6f) return WallType.Door; // 40% porta
            return WallType.Full;                // 40% parede sólida
        }
        private WallType GenerateVerticalWallType()
        {
            float r = Random.value;
            if (r < 0.3f) return WallType.Door; // 30% chance de escada
            return WallType.Full;
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        private void ShuffleDirections(List<Vector2Int> directions)
        {
            // horizontalBias: 0.0 = só verticais, 1.0 = só horizontais
            List<Vector2Int> newOrder = new List<Vector2Int>();

            while (directions.Count > 0)
            {
                Vector2Int chosen;

                // Decide se vai escolher uma direção horizontal ou vertical primeiro
                if (rand.NextDouble() < horizontalBias)
                {
                    // Tenta pegar uma horizontal primeiro (→ ou ←)
                    var horizontals = directions.FindAll(d => d.x != 0);
                    if (horizontals.Count > 0)
                        chosen = horizontals[rand.Next(horizontals.Count)];
                    else
                        chosen = directions[rand.Next(directions.Count)];
                }
                else
                {
                    // Tenta pegar uma vertical (↑ ou ↓)
                    var verticals = directions.FindAll(d => d.y != 0);
                    if (verticals.Count > 0)
                        chosen = verticals[rand.Next(verticals.Count)];
                    else
                        chosen = directions[rand.Next(directions.Count)];
                }

                directions.Remove(chosen);
                newOrder.Add(chosen);
            }

            directions.Clear();
            directions.AddRange(newOrder);
        }
    }
}
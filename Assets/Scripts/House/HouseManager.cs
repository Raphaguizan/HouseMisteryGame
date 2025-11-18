using Guizan.House.Room;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House
{
    public class HouseManager : MonoBehaviour
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
        private float ChanceToDoor2 = .2f;
        [SerializeField, Range(0f,1f)]
        private float horizontalBias = .75f;
        [SerializeField, Range(0f,1f)]
        private float extraDoorPercentage = .2f;

        private RoomController[,] roonsMatrix;
        private System.Random rand = new System.Random();
        private Nullable<Vector2Int> secretRoom = null;

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
            ChooseSecretRoom();
            AddExtraDoors();
            AdjustCamerasBounds();

            Debug.Log("Mapa gerado com sucesso e totalmente conectado!");
        }

        private void ChooseSecretRoom()
        {
            List<Vector2Int> oneDoorRoons = new();
            List<Vector2Int> oneDoorRoonsHorizontal = new();

            for (int x = 0;x < matrixDim.x; x++)
                for(int y = 0;y < matrixDim.y; y++)
                    if(roonsMatrix[x, y].CountDoors() == 1 && !(x == 0 && y == 0))
                        oneDoorRoons.Add(new(x, y));

            for (int i = 0; i < oneDoorRoons.Count; i++)
            {
                RoomController myRoom = roonsMatrix[oneDoorRoons[i].x, oneDoorRoons[i].y];
                WallSide doorSide = myRoom.GetDoorsSide()[0];
                if (doorSide == WallSide.Left || doorSide == WallSide.Right)
                    oneDoorRoonsHorizontal.Add(oneDoorRoons[i]);
            }

            if(oneDoorRoonsHorizontal.Count > 0)
            {
                secretRoom = oneDoorRoonsHorizontal[rand.Next(oneDoorRoonsHorizontal.Count)];
                RoomController myRoom = roonsMatrix[secretRoom.Value.x, secretRoom.Value.y];
                myRoom.name += " S";

                WallSide doorSide = myRoom.GetDoorsSide()[0];
                myRoom.ChangeWallType(doorSide, WallType.Door);

                //Debug.Log($"ChoosenRoom = ({secretRoom.Value.x},{secretRoom.Value.y})");
            }

            if (!secretRoom.HasValue)
                Debug.LogWarning("Não foi gerado um quarto secreto!");
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

            if (UnityEngine.Random.value < ChanceToDoor2)
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

        private void AddExtraDoors()
        {
            int width = matrixDim.x;
            int height = matrixDim.y;
            int totalRooms = width * height;
            int targetExtras = Mathf.RoundToInt(totalRooms * extraDoorPercentage);

            List<Vector2Int> roomPositions = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if(!secretRoom.HasValue || new Vector2Int(x, y) != secretRoom.Value)
                        roomPositions.Add(new Vector2Int(x, y));

            // Embaralha a lista de salas
            for (int i = 0; i < roomPositions.Count; i++)
            {
                int r = rand.Next(i, roomPositions.Count);
                (roomPositions[i], roomPositions[r]) = (roomPositions[r], roomPositions[i]);
            }

            int added = 0;

            foreach (var pos in roomPositions)
            {
                if (added >= targetExtras)
                    break;

                RoomController current = roonsMatrix[pos.x, pos.y];

                // Lista de direções possíveis para abrir nova porta
                List<Vector2Int> possibleDirs = new List<Vector2Int>();

                // CIMA
                if (pos.y < height - 1 && current.GetWallType(WallSide.Ceiling) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x, pos.y + 1) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.up);

                // BAIXO
                if (pos.y > 0 && current.GetWallType(WallSide.Floor) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x, pos.y - 1) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.down);

                // DIREITA
                if (pos.x < width - 1 && current.GetWallType(WallSide.Right) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x + 1, pos.y) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.right);

                // ESQUERDA
                if (pos.x > 0 && current.GetWallType(WallSide.Left) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x - 1, pos.y) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.left);

                if (possibleDirs.Count == 0)
                    continue;

                // Escolhe uma direção aleatória
                Vector2Int dir = possibleDirs[rand.Next(possibleDirs.Count)];
                int nx = pos.x + dir.x;
                int ny = pos.y + dir.y;


                RoomController neighbor = roonsMatrix[nx, ny];

                //Debug.Log($"adicionando porta em ({pos.x},{pos.y}) para o quarto ({nx},{ny})");
                OpenPassage(pos.x, pos.y, nx, ny);
               
                added++;
            }

            Debug.Log($"Adicionadas {added} portas extras ({extraDoorPercentage * 100f}%).");
        }

    }
}
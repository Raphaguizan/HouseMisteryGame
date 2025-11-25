using Guizan.House.Room;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
        [SerializeField, Range(0f,1f), Tooltip("horizontalBias: 0.0 = só verticais, 1.0 = só horizontais")]
        private float horizontalBias = .75f;
        [SerializeField, Range(0f,1f)]
        private float extraDoorPercentage = .2f;

        [Space, SerializeField, Range(0f,1f)]
        private float density = 1f; // 1 = todos os quartos, 0 = só o quarto inicial

        private RoomController[,] roonsMatrix;
        private System.Random rand = new System.Random();
        private Nullable<Vector2Int> secretRoom = null;
        private bool initialized = false;

        public bool IsInitialized => initialized;

        void Start()
        {            
            initialized = false;
            InstantiateRoons();
        }

        private void InstantiateRoons()
        {
            roonsMatrix = new RoomController[matrixDim.x, matrixDim.y];

            // Calcula número alvo de quartos
            int totalCells = matrixDim.x * matrixDim.y;
            int targetCount = Mathf.Clamp(Mathf.RoundToInt(totalCells * density), 1, totalCells);

            // Gera ocupação espalhada e conectada
            bool[,] occupied = GenerateSpreadConnectedOccupancy(targetCount);

            // Determina a ordem de instanciação: instanciamos todas as posições ocupadas
            List<Vector2Int> instantiateOrder = new List<Vector2Int>();
            for (int x = matrixDim.x - 1; x >= 0; x--)
                for (int y = matrixDim.y - 1; y >= 0; y--)
                    if (occupied[x, y])
                        instantiateOrder.Add(new Vector2Int(x, y));

            // garantir ordem determinística/randomizada
            instantiateOrder = instantiateOrder.OrderBy(p => p.x).ThenBy(p => p.y).ToList();

            foreach (var pos in instantiateOrder)
            {
                var newRoom = Instantiate(roomPrefab, roonsParent).transform;
                newRoom.localPosition = new Vector3(pos.x * -horizontalDist, pos.y * -verticalDist);
                newRoom.name = $"Room({pos.x},{pos.y})";
                roonsMatrix[pos.x, pos.y] = newRoom.GetComponent<RoomController>();
            }

            //AddLocalCycles(); // Adiciona ciclos locais após a geração inicial
            CreateShortcuts(4, 7); // distância máxima e prob de preencher intermediários

            // Começa DFS de geração a partir de uma sala existente (escolhe a primeira ocupada que encontrar)
            bool[,] visited = new bool[matrixDim.x, matrixDim.y];
            Vector2Int? first = FindFirstOccupied(occupied);
            if (first.HasValue)
                DFSGenerate(first.Value.x, first.Value.y, visited);
            else
                Debug.LogError("Nenhuma sala ocupada foi gerada!");

            AddExtraDoors();
            AdjustCamerasBounds();

            initialized = true;
            Debug.Log("Mapa gerado com sucesso e totalmente conectado!");
        }
        
        private Vector2Int? FindFirstOccupied(bool[,] occ)
        {
            for (int x = 0; x < matrixDim.x; x++)
                for (int y = 0; y < matrixDim.y; y++)
                    if (occ[x, y])
                        return new Vector2Int(x, y);
            return null;
        }

        // Gera matriz booleana ocupada com distribuição espalhada e garantindo conectividade
        private bool[,] GenerateSpreadConnectedOccupancy(int targetCount)
        {
            bool[,] occ = new bool[matrixDim.x, matrixDim.y];

            // Lista de todas as células
            List<Vector2Int> all = new List<Vector2Int>();
            for (int x = 0; x < matrixDim.x; x++)
                for (int y = 0; y < matrixDim.y; y++)
                    all.Add(new Vector2Int(x, y));

            // Garante que a célula (0,0) sempre exista (spawn seguro)
            Vector2Int forced = new Vector2Int(0, 0);
            occ[forced.x, forced.y] = true;

            // Número de seeds para espalhar — usamos raiz quadrada para espalhar razoavelmente
            int seedsCount = Mathf.Clamp(Mathf.CeilToInt(Mathf.Sqrt(targetCount)), 1, targetCount);

            // Farthest point sampling para escolher seeds espalhadas, já incluindo a forced seed
            List<Vector2Int> seeds = new List<Vector2Int> { forced };

            while (seeds.Count < seedsCount)
            {
                Vector2Int best = default;
                int bestMinDist = -1;
                foreach (var candidate in all)
                {
                    if (seeds.Contains(candidate)) continue;
                    int minDist = int.MaxValue;
                    foreach (var s in seeds)
                    {
                        int d = Mathf.Abs(candidate.x - s.x) + Mathf.Abs(candidate.y - s.y);
                        if (d < minDist) minDist = d;
                    }
                    if (minDist > bestMinDist)
                    {
                        bestMinDist = minDist;
                        best = candidate;
                    }
                }
                seeds.Add(best);
            }

            // Marca seeds como ocupadas
            foreach (var s in seeds)
                occ[s.x, s.y] = true;

            // Conecta seeds via uma árvore geradora mínima (Prim simplificado) usando distância Manhattan
            List<Vector2Int> connected = new List<Vector2Int> { seeds[0] };
            List<Vector2Int> remaining = new List<Vector2Int>(seeds.Skip(1));

            while (remaining.Count > 0)
            {
                int bestDist = int.MaxValue;
                Vector2Int from = default;
                Vector2Int to = default;
                foreach (var c in connected)
                {
                    foreach (var r in remaining)
                    {
                        int d = Mathf.Abs(c.x - r.x) + Mathf.Abs(c.y - r.y);
                        if (d < bestDist)
                        {
                            bestDist = d;
                            from = c;
                            to = r;
                        }
                    }
                }

                // Carve path between 'from' and 'to' (garante conectividade)
                CarvePath(occ, from, to);

                connected.Add(to);
                remaining.Remove(to);
            }

            // Reconstrói lista de ocupados atuais (inclui caminhos)
            List<Vector2Int> occupiedList = new List<Vector2Int>();
            for (int x = 0; x < matrixDim.x; x++)
                for (int y = 0; y < matrixDim.y; y++)
                    if (occ[x, y])
                        occupiedList.Add(new Vector2Int(x, y));

            // Agora expande a partir da borda das células ocupadas até atingir targetCount.
            while (occupiedList.Count < targetCount)
            {
                // Coleta vizinhos possíveis (fronteira) de todas as salas ocupadas
                List<Vector2Int> potentials = new List<Vector2Int>();
                foreach (var p in occupiedList)
                {
                    Vector2Int[] dirs = new Vector2Int[] {
                        Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left
                    };
                    foreach (var d in dirs)
                    {
                        int nx = p.x - d.x;
                        int ny = p.y - d.y;
                        if (nx < 0 || ny < 0 || nx >= matrixDim.x || ny >= matrixDim.y)
                            continue;
                        if (occ[nx, ny])
                            continue;
                        var v = new Vector2Int(nx, ny);
                        if (!potentials.Contains(v))
                            potentials.Add(v);
                    }
                }

                if (potentials.Count == 0)
                    break; // não há mais espaço conectado para crescer

                // Seleção ponderada que incorpora horizontalBias:
                // - calcula distância mínima até as seeds (minD)
                // - calcula diferença média entre componente X e Y (avgDx - avgDy)
                // - score = minD + (orientation * horizontalBiasFactor)
                // Com horizontalBias em [0,1]: 0 => ignora orientação (vertical/horizontal),
                // 1 => favorece fortemente candidatos com maior componente horizontal.
                float bestScore = float.MinValue;
                List<Vector2Int> bestCandidates = new List<Vector2Int>();

                foreach (var cand in potentials)
                {
                    // distância mínima até qualquer seed (mantém espalhamento)
                    int minD = int.MaxValue;
                    float sumDx = 0f;
                    float sumDy = 0f;
                    foreach (var s in seeds)
                    {
                        int d = Mathf.Abs(cand.x - s.x) + Mathf.Abs(cand.y - s.y);
                        if (d < minD) minD = d;
                        sumDx += Mathf.Abs(cand.x - s.x);
                        sumDy += Mathf.Abs(cand.y - s.y);
                    }
                    float avgDx = sumDx / seeds.Count;
                    float avgDy = sumDy / seeds.Count;
                    float orientation = avgDx - avgDy; // >0 prefere horizontal, <0 prefere vertical

                    // normaliza orientation por dimensão média pra evitar escala muito alta em matrizes grandes
                    float diagonalScale = (matrixDim.x + matrixDim.y) * 0.5f;
                    float orientationNorm = orientation / Mathf.Max(1f, diagonalScale);

                    // fator de peso da orientação: quanto maior horizontalBias, mais prioridade a horizontais
                    float orientationWeight = horizontalBias;

                    float score = minD + orientationNorm * orientationWeight * 5f; // 5f = fator empírico para fazer efeito perceptível

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestCandidates.Clear();
                        bestCandidates.Add(cand);
                    }
                    else if (Mathf.Approximately(score, bestScore))
                    {
                        bestCandidates.Add(cand);
                    }
                }

                // Escolhe aleatoriamente entre os melhores candidatos e adiciona — mantém conectividade
                var chosen = bestCandidates[rand.Next(bestCandidates.Count)];
                occ[chosen.x, chosen.y] = true;
                occupiedList.Add(chosen);
            }

            return occ;
        }

        // Carve path entre duas células usando passos intercalados (aleatorizados) e marcando occ = true
        private void CarvePath(bool[,] occ, Vector2Int a, Vector2Int b)
        {
            int x = a.x;
            int y = a.y;
            occ[x, y] = true;

            // Enquanto não chegar em b, escolhe eixo aleatório quando ambos differem
            while (x != b.x || y != b.y)
            {
                if (x != b.x && y != b.y)
                {
                    if (rand.Next(2) == 0)
                        x += Math.Sign(b.x - x);
                    else
                        y += Math.Sign(b.y - y);
                }
                else if (x != b.x)
                    x += Math.Sign(b.x - x);
                else if (y != b.y)
                    y += Math.Sign(b.y - y);

                occ[x, y] = true;
            }
        }

        private void DFSGenerate(int x, int y, bool[,] visited)
        {
            if (roonsMatrix[x, y] == null)
                return;

            visited[x, y] = true;

            // Direções possíveis
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, -1),   // cima
                new Vector2Int(0, 1),  // baixo
                new Vector2Int(-1, 0),   // direita
                new Vector2Int(1, 0)   // esquerda
            };

            // Embaralha as direções
            directions = ShuffleDirections(directions);

            foreach (var dir in directions)
            {
                int nx = x - dir.x;
                int ny = y - dir.y;

                // Pula se for fora da matriz ou sala inexistente
                if (nx < 0 || ny < 0 || nx >= matrixDim.x || ny >= matrixDim.y)
                    continue;
                if (roonsMatrix[nx, ny] == null)
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
            if (x2 < x1)
            {
                // vizinho à direita
                current.ChangeWallType(WallSide.Right, randomType);
                neighbor.ChangeWallType(WallSide.Left, randomType);
            }
            else if (x2 > x1)
            {
                // vizinho à esquerda
                current.ChangeWallType(WallSide.Left, randomType);
                neighbor.ChangeWallType(WallSide.Right, randomType);
            }
            else if (y2 < y1)
            {
                // vizinho acima
                current.ChangeWallType(WallSide.Ceiling, randomType);
                neighbor.ChangeWallType(WallSide.Floor, randomType);
            }
            else if (y2 > y1)
            {
                // vizinho abaixo
                current.ChangeWallType(WallSide.Floor, randomType);
                neighbor.ChangeWallType(WallSide.Ceiling, randomType);
            }
        }

        private void AdjustCamerasBounds()
        {
            for (int x = matrixDim.x - 1; x >= 0; x--)
            {
                for (int y = matrixDim.y - 1; y >= 0; y--)
                {
                    if (roonsMatrix[x, y] == null)
                        continue;

                    if (roonsMatrix[x, y].GetWallType(WallSide.Right) == WallType.Door2)
                    {
                        int xIndex = x;
                        while (xIndex >= 0 && (!roonsMatrix[xIndex, y].HasCollider))
                        {
                            xIndex--;
                        }
                        if (xIndex >= 0 && x - 1 < matrixDim.x && roonsMatrix[x-1, y] != null)
                        {
                            roonsMatrix[xIndex, y].AdaptColliderPointsToRight(roonsMatrix[x-1, y]);                    
                        }
                    }
                }
            }
        }

        private List<Vector2Int> ShuffleDirections(List<Vector2Int> directions)
        {
            // horizontalBias: 0.0 = só verticais, 1.0 = só horizontais
            List<Vector2Int> newOrder = new List<Vector2Int>();
            List<Vector2Int> directionsList = directions;

            while (directionsList.Count > 0)
            {
                Vector2Int chosen;

                // Decide se vai escolher uma direção horizontal ou vertical primeiro
                if (rand.NextDouble() < horizontalBias)
                {
                    // Tenta pegar uma horizontal primeiro (→ ou ←)
                    var horizontals = directionsList.FindAll(d => d.x != 0);
                    if (horizontals.Count > 0)
                        chosen = horizontals[rand.Next(horizontals.Count)];
                    else
                        chosen = directionsList[rand.Next(directionsList.Count)];
                }
                else
                {
                    // Tenta pegar uma vertical (↑ ou ↓)
                    var verticals = directionsList.FindAll(d => d.y != 0);
                    if (verticals.Count > 0)
                        chosen = verticals[rand.Next(verticals.Count)];
                    else
                        chosen = directionsList[rand.Next(directionsList.Count)];
                }

                directionsList.Remove(chosen);
                newOrder.Add(chosen);
            }

            return newOrder;
        }

        private void AddExtraDoors()
        {
            int width = matrixDim.x;
            int height = matrixDim.y;

            // Calcula portas extras com base no número real de quartos criados
            int createdRooms = CountOccupied();

            int targetExtras = Mathf.RoundToInt(createdRooms * extraDoorPercentage);

            List<Vector2Int> roomPositions = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (roonsMatrix[x, y] != null && (!secretRoom.HasValue || new Vector2Int(x, y) != secretRoom.Value))
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
                if (current == null)
                    continue;

                // Lista de direções possíveis para abrir nova porta
                List<Vector2Int> possibleDirs = new List<Vector2Int>();

                // CIMA
                if (pos.y > 0 && current.GetWallType(WallSide.Ceiling) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x, pos.y + 1) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.up);

                // BAIXO
                if (pos.y < height - 1  && current.GetWallType(WallSide.Floor) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x, pos.y - 1) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.down);

                // DIREITA
                if (pos.x > 0 && current.GetWallType(WallSide.Right) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x + 1, pos.y) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.right);

                // ESQUERDA
                if (pos.x < width - 1 && current.GetWallType(WallSide.Left) == WallType.Full && (!secretRoom.HasValue || new Vector2Int(pos.x - 1, pos.y) != secretRoom.Value))
                    possibleDirs.Add(Vector2Int.left);

                if (possibleDirs.Count <= 1)
                    continue;

                
                // Escolhe uma direção aleatória
                var possibleDirsShuffled = ShuffleDirections(possibleDirs);
                int nx, ny;
                nx = ny = -1;

                for (int i = 0; i < possibleDirsShuffled.Count; i++)
                {
                    Vector2Int dir = possibleDirsShuffled[i];
                    int nx_aux = pos.x - dir.x;
                    int ny_aux = pos.y - dir.y;

                    if (nx_aux >= 0 && ny_aux >= 0 && nx_aux < width && ny_aux < height && roonsMatrix[nx_aux, ny_aux] != null)
                    {
                        nx = nx_aux;
                        ny = ny_aux;
                        break;
                    }
                }

                if (nx == -1 && ny == -1)
                    continue;

                OpenPassage(pos.x, pos.y, nx, ny);
                //Debug.Log($"Porta extra adicionada entre ({pos.x},{pos.y}) e ({nx},{ny})");

                added++;
            }

            Debug.Log($"Adicionadas {added} portas extras ({extraDoorPercentage * 100f}% de {createdRooms} comodos).");
        }

        /// <summary>
        /// Attempts to create local cycles by connecting adjacent rooms in the matrix with new passages, based on the
        /// specified connection probability and horizontal bias.
        /// </summary>
        /// <remarks>The method favors horizontal or vertical connections according to the horizontal bias
        /// setting. Only rooms that are directly adjacent and not already connected by a passage are considered. This
        /// can increase the overall connectivity and loopiness of the room layout.</remarks>
        /// <param name="connectProbability">The base probability, in the range [0, 1], of creating a connection between two adjacent rooms. Higher
        /// values increase the likelihood of additional passages.</param>
        private void AddLocalCycles(float connectProbability = 0.15f)
        {
            int width = matrixDim.x;
            int height = matrixDim.y;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (roonsMatrix[x, y] == null) continue;

                    // Apenas checa direita e cima para evitar duplicação (pode ajustar)
                    var neighbors = new List<Vector2Int>
                    {
                        new Vector2Int(x + 1, y), // direita
                        new Vector2Int(x, y + 1)  // cima
                    };

                    foreach (var n in neighbors)
                    {
                        if (n.x < 0 || n.y < 0 || n.x >= width || n.y >= height) continue;
                        if (roonsMatrix[n.x, n.y] == null) continue;

                        bool isHorizontal = (n.x != x);
                        // Ajusta probabilidade pela orientação pedida
                        float orientFactor = isHorizontal ? horizontalBias : (1f - horizontalBias);
                        float finalProb = connectProbability * Mathf.Clamp01(orientFactor);

                        if (rand.NextDouble() < finalProb)
                        {
                            // Só conecta se já não houver porta (testa parede atual)
                            // Considera walls do RoomController através de GetWallType
                            // Se estiver Full, abre a passagem
                            WallSide sideFrom = isHorizontal ? WallSide.Right : WallSide.Ceiling;
                            WallType wallType = roonsMatrix[x, y].GetWallType(sideFrom);
                            if (wallType == WallType.Full)
                            {
                                OpenPassage(x, y, n.x, n.y);
                                Debug.Log($"Ciclo local adicionado entre ({x},{y}) e ({n.x},{n.y})");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates additional shortcut rooms between existing rooms to improve connectivity within the layout. Shortcut
        /// rooms are placed based on the specified minimum and maximum Manhattan distances between room pairs.
        /// </summary>
        /// <remarks>Shortcut rooms are only created if the budget, determined by the number of existing
        /// rooms and the configured extra door percentage, is greater than zero. The method avoids creating shortcuts
        /// between rooms that are already well-connected and only adds intermediate rooms when the shortest existing
        /// path is significantly longer than the direct Manhattan distance. This helps optimize navigation and reduce
        /// unnecessary detours in the generated layout.</remarks>
        /// <param name="minDistance">The minimum Manhattan distance, in grid units, between pairs of rooms considered for shortcut creation. Must
        /// be at least 1.</param>
        /// <param name="maxDistance">The maximum Manhattan distance, in grid units, between pairs of rooms considered for shortcut creation. Must
        /// be greater than or equal to <paramref name="minDistance"/>.</param>
        private void CreateShortcuts(int minDistance = 2, int maxDistance = 5)
        {
            int width = matrixDim.x;
            int height = matrixDim.y;

            if (minDistance < 1) minDistance = 1;
            if (maxDistance < minDistance) maxDistance = minDistance;

            int createdRooms = CountOccupied();
            int budget = Mathf.RoundToInt(createdRooms * extraDoorPercentage);
            if (budget <= 0) return;

            // Lista atual de salas ocupadas
            List<Vector2Int> occupied = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (roonsMatrix[x, y] != null)
                        occupied.Add(new Vector2Int(x, y));

            if (occupied.Count < 2) return;

            // Gera pares candidatos (i<j) que respeitam min/max distance
            var pairs = new List<(Vector2Int a, Vector2Int b, int manhattan)>();
            for (int i = 0; i < occupied.Count; i++)
            {
                for (int j = i + 1; j < occupied.Count; j++)
                {
                    var a = occupied[i];
                    var b = occupied[j];
                    int manh = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
                    if (manh >= minDistance && manh <= maxDistance)
                        pairs.Add((a, b, manh));
                }
            }

            // Embaralha pares para dispersão
            for (int i = 0; i < pairs.Count; i++)
            {
                int r = rand.Next(i, pairs.Count);
                var tmp = pairs[i];
                pairs[i] = pairs[r];
                pairs[r] = tmp;
            }

            // Função local: BFS para distância de caminho entre salas existentes (somente por salas já criadas)
            int GraphDistance(Vector2Int start, Vector2Int goal)
            {
                if (start == goal) return 0;
                var q = new Queue<Vector2Int>();
                var seen = new HashSet<Vector2Int> { start };
                q.Enqueue(start);
                int depth = 0;
                while (q.Count > 0)
                {
                    int cnt = q.Count;
                    depth++;
                    for (int k = 0; k < cnt; k++)
                    {
                        var cur = q.Dequeue();
                        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                        foreach (var d in dirs)
                        {
                            int nx = cur.x + d.x;
                            int ny = cur.y + d.y;
                            if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                            var nb = new Vector2Int(nx, ny);
                            if (seen.Contains(nb)) continue;
                            if (roonsMatrix[nx, ny] == null) continue; // só percorre por salas existentes
                            if (nb == goal) return depth;
                            seen.Add(nb);
                            q.Enqueue(nb);
                        }
                    }
                }
                return int.MaxValue; // não alcançável
            }

            // Processa pares e cria quartos intermediários quando necessário
            foreach (var pair in pairs)
            {
                if (budget <= 0) break;

                var a = pair.a;
                var b = pair.b;
                int manh = pair.manhattan;

                int graphDist = GraphDistance(a, b);

                // Critério: se caminho via salas existentes for muito maior que a Manhattan, adiciona intermediários.
                // Ajuste a sensibilidade conforme desejado. Aqui: se graphDist é int.MaxValue (desconexo) ou
                // graphDist > manh + 2 (excesso de passos), então aproximamos via caminho direto.
                if (graphDist == int.MaxValue || graphDist > manh + 2)
                {
                    // Gera caminho direto (L-shape) entre a e b
                    List<Vector2Int> path = new List<Vector2Int>();
                    int cx = a.x;
                    int cy = a.y;
                    path.Add(new Vector2Int(cx, cy));
                    bool doXFirst = rand.NextDouble() < 0.5;
                    if (doXFirst)
                    {
                        while (cx != b.x)
                        {
                            cx += Math.Sign(b.x - cx);
                            path.Add(new Vector2Int(cx, cy));
                        }
                        while (cy != b.y)
                        {
                            cy += Math.Sign(b.y - cy);
                            path.Add(new Vector2Int(cx, cy));
                        }
                    }
                    else
                    {
                        while (cy != b.y)
                        {
                            cy += Math.Sign(b.y - cy);
                            path.Add(new Vector2Int(cx, cy));
                        }
                        while (cx != b.x)
                        {
                            cx += Math.Sign(b.x - cx);
                            path.Add(new Vector2Int(cx, cy));
                        }
                    }

                    // Instancia apenas células intermediárias (não as extremidades) que estejam vazias,
                    // consumindo do budget.
                    for (int i = 1; i < path.Count - 1; i++)
                    {
                        var p = path[i];
                        if (roonsMatrix[p.x, p.y] == null)
                        {
                            var newRoom = Instantiate(roomPrefab, roonsParent).transform;
                            newRoom.localPosition = new Vector3(p.x * -horizontalDist, p.y * -verticalDist);
                            newRoom.name = $"Room({p.x},{p.y})";
                            roonsMatrix[p.x, p.y] = newRoom.GetComponent<RoomController>();
                            occupied.Add(new Vector2Int(p.x, p.y));
                            budget--;
                            //Debug.Log($"Criado comodo intermediário em ({p.x},{p.y}) para aproximar {a} <-> {b}. Orçamento restante: {budget}");
                        }
                    }
                }
            }

            Debug.Log($"CreateShortcuts finalizado. Novos quartos criados: {Mathf.RoundToInt(createdRooms * extraDoorPercentage) - budget} (orçamento inicial {Mathf.RoundToInt(createdRooms * extraDoorPercentage)}).");
        }

        public List<RoomController> GetRooms()
        {
            List<RoomController> resp = new();
            for (int x = 0; x < matrixDim.x; x++)
                for (int y = 0; y < matrixDim.y; y++)
                    if (roonsMatrix[x, y] != null)
                        resp.Add(roonsMatrix[x, y]);
                    
            return resp;
        }

        public int CountOccupied()
        {
            return GetRooms().Count;
        }
    }
}
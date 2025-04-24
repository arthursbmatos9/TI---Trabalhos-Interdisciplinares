using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;  // Use UnityEngine.TextMeshPro para TextMeshPro
using TMPro;
using static RushHourPuzzle.Graph;
using static GerarMapasNamespace.GerarMapaAleatorio;
using System.IO;

public class RushHourPuzzle : MonoBehaviour
{
    private Graph graph = new Graph();
    public GameObject asfalto_estacionamento;
    public GameObject veiculos;
    private Graph.SolutionPath lastSolution;

    [SerializeField] private TMP_Text uiText2;
    [SerializeField] private TMP_Text uiText2Copia;

    public void Start()
    {
       //
    }

    public Graph.SolutionPath GetLastSolution()
    {
        return lastSolution;
    }


    public void ResetAndSolve()
    {
        ResetToInitialState();
        ApplySavedSolution();
    }

    public void ApplySavedSolution()
    {
        try
        {
            // Carrega a solução salva
            string solutionJson = File.ReadAllText(Path.Combine(Application.temporaryCachePath, "RushHourTemp/solution.dat"));
            var savedSolution = JsonUtility.FromJson<SerializableSolution>(solutionJson);

            // Carrega o estado inicial salvo
            string initialStateJson = File.ReadAllText(Path.Combine(Application.temporaryCachePath, "RushHourTemp/initial_state.dat"));
            var wrapper = JsonUtility.FromJson<SerializableVehicleWrapper>(initialStateJson);

            if (savedSolution == null || savedSolution.Moves == null)
            {
                Debug.LogError("Não foi possível carregar a solução salva!");
                return;
            }

            // Converte o estado inicial serializado para GameState
            List<Graph.Vehicle> vehicles = wrapper.vehicles.Select(sv =>
                new Graph.Vehicle(sv.X, sv.Y, sv.IsHorizontal, sv.Length, sv.Type, sv.IsMain, sv.IsObstacle)
            ).ToList();

            var initialState = Graph.CreateInitialState(vehicles);

            // Cria um SolutionPath com os dados salvos
            var solution = new Graph.SolutionPath
            {
                Moves = savedSolution.Moves,
                TotalMoves = savedSolution.TotalMoves
            };

            // Usa o método existente
            StartCoroutine(ApplySolution(solution, initialState));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao aplicar solução: {e.Message}");
        }
    }




    public void ResetToInitialState()
    {
        try
        {
            string json = File.ReadAllText(Path.Combine(Application.temporaryCachePath, "RushHourTemp/initial_state.dat"));
            var wrapper = JsonUtility.FromJson<SerializableVehicleWrapper>(json);

            if (wrapper == null || wrapper.vehicles == null)
            {
                Debug.LogError("Não foi possível carregar o estado inicial!");
                return;
            }

            foreach (var serializedVehicle in wrapper.vehicles)
            {
                Transform vehicleTransform = veiculos.transform.Find(serializedVehicle.Type);
                if (vehicleTransform == null)
                {
                    Debug.LogError($"Não foi possível encontrar o veículo: {serializedVehicle.Type}");
                    continue;
                }

                // Simplesmente restaura a posição e rotação originais
                vehicleTransform.position = serializedVehicle.Position;
                vehicleTransform.rotation = serializedVehicle.Rotation;
            }

            Debug.Log("Tabuleiro resetado para o estado inicial!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao resetar tabuleiro: {e.Message}");
        }
    }


    public int SolvePuzzle(List<Graph.Vehicle> list)
    {
        if (uiText2 == null)
        {
            uiText2 = GameObject.Find("textogui").GetComponent<TMP_Text>();
            if (uiText2 == null)
            {
                Debug.LogError("Não foi possível encontrar o componente Text.");
            }
        }
        if (uiText2Copia == null)
        {
            uiText2Copia = GameObject.Find("textoguiCopia").GetComponent<TMP_Text>();
            if (uiText2Copia == null)
            {
                Debug.LogError("Não foi possível encontrar o componente Text Copia.");
            }
        }
        List<Graph.Vehicle> vehicles = list;

        Debug.Log("Initial board state:");
        var initialState = Graph.CreateInitialState(vehicles);
        Debug.Log(initialState.GetBoardState());

        Debug.Log("Solving puzzle...");
        SolutionPath solution = graph.SolveGame(initialState);
        Debug.Log(solution == null);
        if (solution != null)
        {
            uiText2.text=$"{solution.TotalMoves}" + " passos";
            uiText2Copia.text=$"Min: {solution.TotalMoves}";
            Debug.Log("\nSolution steps:");

            for (int i = 0; i < solution.Moves.Count; i++)
            {
                Debug.Log($"\nStep {i + 1}: {solution.Moves[i]}");
                Debug.Log(solution.BoardStates[i + 1]);

            }
            //var x = ApplySolution(solution, initialState);
            //StartCoroutine(x);
            lastSolution = solution;
            return solution.Moves.Count;
        }
        else
        {
            Debug.Log("No solution found!");
            return 0;
        }
    }

    private Graph.Vehicle FindVehicleBySymbol(char symbol, List<Graph.Vehicle> vehicles)
    {
        foreach (var v in vehicles)
        {
            if (v.GetDisplaySymbol() == symbol) return v;
        }
        return null;
    }

    public IEnumerator ApplySolution(Graph.SolutionPath solution, Graph.GameState initialState)
    {
        if (solution == null || solution.Moves == null)
        {
            Debug.LogError("Solution ou seus movimentos estão nulos!");
            yield break;
        }

        float largura = asfalto_estacionamento.GetComponent<Renderer>().bounds.size.x / 10;
        float altura = asfalto_estacionamento.GetComponent<Renderer>().bounds.size.y / 6;

        foreach (string move in solution.Moves)
        {
            string[] parts = move.Split(' ');
            char vehicleSymbol = parts[1][0];
            string direction = parts[2];

            Graph.Vehicle vehicle = FindVehicleBySymbol(vehicleSymbol, initialState.Vehicles); // Usa initialState aqui
            if (vehicle == null)
            {
                Debug.LogError("Ve�culo n�o encontrado: " + vehicleSymbol);
                continue;
            }
            Debug.Log($"Buscando veículo do tipo: {vehicle.Type}");
            GameObject vehicleObject = veiculos.transform.Find(vehicle.Type).gameObject;
            if (vehicleObject == null)
            {
                Debug.LogError("GameObject do ve�culo n�o encontrado: " + vehicle.Type);
                continue;
            }

            float displacementX = 0;
            float displacementY = 0;

            if (direction == "right") displacementX = largura;
            else if (direction == "left") displacementX = -largura;
            else if (direction == "up") displacementY = altura;
            else if (direction == "down") displacementY = -altura;

            Vector3 newPosition = vehicleObject.transform.position + new Vector3(displacementX, displacementY, 0);
            StartCoroutine(MoveVehicleSmooth(vehicleObject, newPosition)); // Movimento suave

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveVehicleSmooth(GameObject vehicle, Vector3 targetPosition)
    {
        float duration = 0.3f; // Dura��o da anima��o
        float elapsedTime = 0;

        Vector3 startingPos = vehicle.transform.position;
        while (elapsedTime < duration)
        {
            Debug.Log($"Movendo veículo de {startingPos} para {targetPosition}");

            vehicle.transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Aguarda o pr�ximo frame
        }
        vehicle.transform.position = targetPosition; // Garante a posi��o final correta
    }



    // Helper method to create custom puzzle configurations
    public void SolveCustomPuzzle(List<Graph.Vehicle> vehicles)
    {
        var initialState = Graph.CreateInitialState(vehicles);
        Debug.Log("Initial board state:");
        Debug.Log(initialState.GetBoardState());

        Debug.Log("Solving puzzle...");
        var solution = graph.SolveGame(initialState);

        if (solution != null)
        {
            Debug.Log($"Solution found in {solution.TotalMoves} moves!");
            Debug.Log("\nSolution steps:");

            for (int i = 0; i < solution.Moves.Count; i++)
            {
                Debug.Log($"\nStep {i + 1}: {solution.Moves[i]}");
                Debug.Log(solution.BoardStates[i + 1]);
            }
        }
        else
        {
            Debug.Log("No solution found!");
        }
    }

    public class Graph
    {


        public class Vehicle
        {
            // Campos estáticos
            private static readonly Dictionary<string, char> VehicleSymbols = new Dictionary<string, char>()
    {
        {"carro_azul", 'Z'},
        {"carro_verde_escuro", 'E'},
        {"carro_verde_claro", 'C'},
        {"carro_cinza_grande", 'G'},
        {"carro_amarelo_fraco", 'F'},
        {"carro_amarelo_forte", 'O'},
        {"carro_vermelho_target", 'R'},
        {"bus_branco", 'B'},
        {"bus_cinza", 'S'},
        {"bus_amarelo", 'Y'},
        {"truck_marrom", 'M'},
        {"truck_vermelho", 'V'},
        {"truck_azul_claro", 'L'},
        {"truck_azul_escuro", 'D'},
        {"truck_preto", 'P'},
        {"carro_cinza_pequeno", 'Q'},
        {"carro_amarelo_medio", 'I'},
        {"obs1", '#'},
        {"obs2", '#'},
        {"obs3", '#'}
    };

            // Propriedades
            public int X { get; set; }
            public int Y { get; set; }
            public bool IsHorizontal { get; set; }
            public int Length { get; set; }
            public bool IsMain { get; set; }
            public string Type { get; }
            public bool IsObstacle { get; }
            public int Id { get; set; } // Novo: ID para identificação rápida

            // Cache do símbolo
            private readonly char _symbol;

            public Vehicle(int x, int y, bool isHorizontal, int length, string type, bool isMain = false, bool isObstacle = false)
            {
                X = x;
                Y = y;
                IsHorizontal = isHorizontal;
                Length = length;
                Type = type;
                IsMain = isMain;
                IsObstacle = isObstacle;

                // Cache do símbolo na construção
                _symbol = VehicleSymbols.TryGetValue(type, out char symbol) ? symbol : '?';
            }

            public char GetDisplaySymbol() => _symbol;

            public Vehicle Clone() => new Vehicle(X, Y, IsHorizontal, Length, Type, IsMain, IsObstacle) { Id = this.Id };

            // Override do Equals e GetHashCode para melhor performance em HashSet e Dictionary
            public override bool Equals(object obj)
            {
                if (obj is Vehicle other)
                {
                    return X == other.X &&
                           Y == other.Y &&
                           IsHorizontal == other.IsHorizontal &&
                           Length == other.Length &&
                           Type == other.Type &&
                           IsMain == other.IsMain &&
                           IsObstacle == other.IsObstacle;
                }
                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + X;
                    hash = hash * 23 + Y;
                    hash = hash * 23 + IsHorizontal.GetHashCode();
                    hash = hash * 23 + Length;
                    hash = hash * 23 + Type.GetHashCode();
                    hash = hash * 23 + IsMain.GetHashCode();
                    hash = hash * 23 + IsObstacle.GetHashCode();
                    return hash;
                }
            }
        }

        public class GameState : IEquatable<GameState>
        {
            public List<Vehicle> Vehicles { get; }
            public int MovesCount { get; }
            public GameState Parent { get; }
            public string Move { get; set; }

            private int? _hashCode;

            public GameState(List<Vehicle> vehicles, int movesCount, GameState parent = null, string move = null)
            {
                Vehicles = vehicles ?? throw new ArgumentNullException(nameof(vehicles));
                MovesCount = movesCount;
                Parent = parent;
                Move = move;
            }

            public string GetBoardState()
            {
                var boardBuilder = new StringBuilder();
                char[,] board = new char[BOARD_ROWS, BOARD_COLS];
            
                for (int i = 0; i < BOARD_ROWS; i++)
                    for (int j = 0; j < BOARD_COLS; j++)
                        board[i, j] = '.';
            
                foreach (var vehicle in Vehicles)
                {
                    char symbol = vehicle.GetDisplaySymbol();
            
                    for (int i = 0; i < vehicle.Length; i++)
                    {
                        if (vehicle.IsHorizontal)
                            board[vehicle.Y, vehicle.X + i] = symbol;
                        else
                            board[vehicle.Y + i, vehicle.X] = symbol;
                    }
                }
            
                boardBuilder.AppendLine("  0 1 2 3 4 5 6 7 8 9");
                boardBuilder.AppendLine("  -------------------");
            
                for (int i = BOARD_ROWS -1; i >= 0; i--)
                {
                    boardBuilder.Append($"{i}|");
                    for (int j = 0; j < BOARD_COLS; j++)
                    {
                        boardBuilder.Append($"{board[i, j]} ");
                    }
                    if (i == EXIT_ROW)
                        boardBuilder.Append("EXIT");
                    boardBuilder.AppendLine();
                }
            
                return boardBuilder.ToString();
            }

            public bool Equals(GameState other)
            {
                if (other == null) return false;
                if (Vehicles.Count != other.Vehicles.Count) return false;

                for (int i = 0; i < Vehicles.Count; i++)
                {
                    if (Vehicles[i].X != other.Vehicles[i].X ||
                        Vehicles[i].Y != other.Vehicles[i].Y)
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                if (_hashCode.HasValue) return _hashCode.Value;

                unchecked
                {
                    int hash = 17;
                    foreach (var vehicle in Vehicles)
                    {
                        hash = hash * 31 + vehicle.X;
                        hash = hash * 31 + vehicle.Y;
                    }
                    _hashCode = hash;
                    return hash;
                }
            }

            public GameState Clone()
            {
                return new GameState(
                    Vehicles.Select(v => v.Clone()).ToList(),
                    MovesCount,
                    this,
                    Move
                );
            }
        }

        public class PriorityQueue<T>
        {
            private readonly List<(T Item, int Priority)> _heap;
            private readonly int _initialCapacity;

            public PriorityQueue(int initialCapacity = 11)
            {
                _initialCapacity = initialCapacity;
                _heap = new List<(T, int)>(initialCapacity);
            }

            public void Enqueue(T item, int priority)
            {
                _heap.Add((item, priority));
                SiftUp(_heap.Count - 1);
            }

            public T Dequeue()
            {
                if (IsEmpty())
                    throw new InvalidOperationException("Queue is empty");

                T result = _heap[0].Item;
                int lastIndex = _heap.Count - 1;

                // Move último elemento para a raiz e remove o último
                _heap[0] = _heap[lastIndex];
                _heap.RemoveAt(lastIndex);

                if (!IsEmpty())
                {
                    SiftDown(0);
                }

                return result;
            }

            public bool IsEmpty() => _heap.Count == 0;

            public void Clear() => _heap.Clear();

            private void SiftUp(int index)
            {
                var item = _heap[index];

                while (index > 0)
                {
                    int parentIndex = (index - 1) >> 1;
                    var parent = _heap[parentIndex];

                    if (item.Priority >= parent.Priority)
                        break;

                    _heap[index] = parent;
                    index = parentIndex;
                }

                _heap[index] = item;
            }

            private void SiftDown(int index)
            {
                var item = _heap[index];
                int lastIndex = _heap.Count - 1;

                while (true)
                {
                    int leftChildIndex = (index << 1) + 1;
                    if (leftChildIndex > lastIndex)
                        break;

                    int rightChildIndex = leftChildIndex + 1;
                    int minChildIndex = leftChildIndex;

                    if (rightChildIndex <= lastIndex && _heap[rightChildIndex].Priority < _heap[leftChildIndex].Priority)
                    {
                        minChildIndex = rightChildIndex;
                    }

                    if (item.Priority <= _heap[minChildIndex].Priority)
                        break;

                    _heap[index] = _heap[minChildIndex];
                    index = minChildIndex;
                }

                _heap[index] = item;
            }

            public int Count => _heap.Count;
        }

        public class SolutionPath
        {
            public List<string> Moves { get; set; }
            public List<string> BoardStates { get; set; }
            public int TotalMoves { get; set; }

            public SolutionPath()
            {
                Moves = new List<string>();
                BoardStates = new List<string>();
                TotalMoves = 0;
            }

            public SolutionPath(List<string> moves, int totalMoves)
            {
                Moves = moves;
                TotalMoves = totalMoves;
                BoardStates = new List<string>();
            }
        }

        private const int BOARD_ROWS = 6;
        private const int BOARD_COLS = 10;
        private const int EXIT_ROW = 2;

        public static GameState CreateInitialState(List<Vehicle> vehicles)
        {
            return new GameState(vehicles, 0);
        }

        public SolutionPath SolveGame(GameState initialState)
        {
            return SolveGame(initialState, 2f); // Tempo limite padrão de 5 segundos
        }

        public SolutionPath SolveGame(GameState initialState, float timeoutSeconds = 10f)
        {
            if (initialState == null)
                throw new ArgumentNullException(nameof(initialState));
            if (!initialState.Vehicles.Any(v => v.IsMain))
                throw new InvalidOperationException("Initial state must contain a main vehicle");

            // Cache do veículo principal para uso na heurística
            Vehicle mainVehicle = initialState.Vehicles.First(v => v.IsMain);
            int exitRow = mainVehicle.Y;

            // Pré-calcula o tamanho aproximado das coleções baseado no tamanho do tabuleiro
            int estimatedStates = BOARD_ROWS * BOARD_COLS * initialState.Vehicles.Count;
            var openSet = new PriorityQueue<GameState>(Math.Min(estimatedStates, 10000));
            var closedSet = new HashSet<GameState>(estimatedStates);
            var costs = new Dictionary<GameState, int>(estimatedStates);

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // Pré-aloca o buffer do tabuleiro para reutilização
            bool[,] boardBuffer = new bool[BOARD_ROWS, BOARD_COLS];

            openSet.Enqueue(initialState, FastHeuristic(initialState, mainVehicle, exitRow));
            costs[initialState] = 0;

            while (!openSet.IsEmpty())
            {
                if (stopwatch.ElapsedMilliseconds > timeoutSeconds * 1000)
                {
                    return null;
                }

                var current = openSet.Dequeue();

                // Otimização: Verifica o estado objetivo mais cedo
                if (IsGoalState(current, mainVehicle))
                {
                    return CreateSolutionPath(current);
                }

                if (!closedSet.Add(current)) // Tenta adicionar e verifica se já existe em uma operação
                {
                    continue;
                }

                // Gera movimentos possíveis com buffer reutilizável
                foreach (var nextState in GetPossibleMovesOptimized(current, boardBuffer))
                {
                    int tentativeCost = costs[current] + 1;

                    // Otimização: Combina verificações para reduzir lookups
                    if (!costs.TryGetValue(nextState, out int currentCost) || tentativeCost < currentCost)
                    {
                        costs[nextState] = tentativeCost;
                        int priority = tentativeCost + FastHeuristic(nextState, mainVehicle, exitRow);
                        openSet.Enqueue(nextState, priority);
                    }
                }
            }

            return null;
        }

        // Heurística otimizada que evita LINQ e usa informações cacheadas
        private int FastHeuristic(GameState state, Vehicle mainVehicle, int exitRow)
        {
            var currentMain = state.Vehicles[mainVehicle.Id]; // Assume que a ordem dos veículos é preservada
            int distanceToExit = BOARD_COLS - (currentMain.X + currentMain.Length);
            int blockingVehicles = FastCountBlockingVehicles(state, currentMain, exitRow);

            return distanceToExit + (blockingVehicles * 2);
        }

        private int FastCountBlockingVehicles(GameState state, Vehicle mainCar, int exitRow)
        {
            int count = 0;
            int pathStart = mainCar.X + mainCar.Length;
            var vehicles = state.Vehicles;

            // Usa for em vez de foreach para melhor performance
            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicle = vehicles[i];
                if (!vehicle.IsMain && !vehicle.IsHorizontal &&
                    vehicle.X > pathStart && vehicle.Y <= exitRow &&
                    vehicle.Y + vehicle.Length > exitRow)
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsGoalState(GameState state, Vehicle mainVehicle)
        {
            var currentMain = state.Vehicles[mainVehicle.Id];
            return currentMain.X + currentMain.Length >= BOARD_COLS;
        }

        private List<GameState> GetPossibleMovesOptimized(GameState state, bool[,] boardBuffer)
        {
            var possibleMoves = new List<GameState>(state.Vehicles.Count * 2); // Pre-aloca para o pior caso
            CreateBoardOptimized(state, boardBuffer); // Reutiliza o buffer do tabuleiro

            for (int i = 0; i < state.Vehicles.Count; i++)
            {
                var vehicle = state.Vehicles[i];
                if (vehicle.IsObstacle) continue;

                // Usa o símbolo correto do veículo
                char vehicleSymbol = vehicle.GetDisplaySymbol();

                if (CanMoveOptimized(vehicle, boardBuffer, 1) && !vehicle.IsObstacle)
                {
                    var newState = state.Clone();
                    var newVehicle = newState.Vehicles[i];
                    if (vehicle.IsHorizontal)
                    {
                        newVehicle.X += 1;
                        newState.Move = $"Move {vehicleSymbol} right";
                    }
                    else
                    {
                        newVehicle.Y += 1;
                        newState.Move = $"Move {vehicleSymbol} up";
                    }
                    possibleMoves.Add(newState);
                }

                if (CanMoveOptimized(vehicle, boardBuffer, -1) && !vehicle.IsObstacle)
                {
                    var newState = state.Clone();
                    var newVehicle = newState.Vehicles[i];
                    if (vehicle.IsHorizontal)
                    {
                        newVehicle.X -= 1;
                        newState.Move = $"Move {vehicleSymbol} left";
                    }
                    else
                    {
                        newVehicle.Y -= 1;
                        newState.Move = $"Move {vehicleSymbol} down";
                    }
                    possibleMoves.Add(newState);
                }
            }

            return possibleMoves;
        }

        private void CreateBoardOptimized(GameState state, bool[,] board)
        {
            // Limpa o buffer rapidamente
            Array.Clear(board, 0, board.Length);

            // Preenche o tabuleiro com os veículos
            foreach (var vehicle in state.Vehicles)
            {
                if (vehicle.IsHorizontal)
                {
                    for (int x = vehicle.X; x < vehicle.X + vehicle.Length; x++)
                    {
                        board[vehicle.Y, x] = true;
                    }
                }
                else
                {
                    for (int y = vehicle.Y; y < vehicle.Y + vehicle.Length; y++)
                    {
                        board[y, vehicle.X] = true;
                    }
                }
            }
        }

        private bool CanMoveOptimized(Vehicle vehicle, bool[,] board, int direction)
        {
            if (vehicle.Type == "obs1" || vehicle.Type == "obs2" || vehicle.Type == "obs3") return false;

            if (vehicle.IsHorizontal)
            {
                int newX = direction > 0 ? vehicle.X + vehicle.Length : vehicle.X - 1;
                return newX >= 0 && newX < BOARD_COLS && !board[vehicle.Y, newX];
            }
            else
            {
                int newY = direction > 0 ? vehicle.Y + vehicle.Length : vehicle.Y - 1;
                return newY >= 0 && newY < BOARD_ROWS && !board[newY, vehicle.X];
            }
        }

        private bool IsVehicleWithinBounds(Vehicle vehicle)
        {
            if (vehicle.IsHorizontal)
            {
                return vehicle.X >= 0 &&
                       vehicle.X + vehicle.Length <= BOARD_COLS &&
                       vehicle.Y >= 0 &&
                       vehicle.Y < BOARD_ROWS;
            }
            else
            {
                return vehicle.X >= 0 &&
                       vehicle.X < BOARD_COLS &&
                       vehicle.Y >= 0 &&
                       vehicle.Y + vehicle.Length <= BOARD_ROWS;
            }
        }

        private int Heuristic(GameState state)
        {
            var mainVehicle = state.Vehicles.FirstOrDefault(v => v.IsMain);
            if (mainVehicle == null)
            {
                throw new InvalidOperationException("No main vehicle found in the current game state.");
            }

            int distanceToExit = BOARD_COLS - (mainVehicle.X + mainVehicle.Length);
            int blockingVehicles = CountBlockingVehicles(state, mainVehicle);

            return distanceToExit + (blockingVehicles * 2);
        }

        private int CountBlockingVehicles(GameState state, Vehicle mainCar)
        {
            int count = 0;
            int pathStart = mainCar.X + mainCar.Length;

            foreach (var vehicle in state.Vehicles)
            {
                if (!vehicle.IsMain && !vehicle.IsHorizontal &&
                    vehicle.X > pathStart && vehicle.Y <= EXIT_ROW &&
                    vehicle.Y + vehicle.Length > EXIT_ROW)
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsGoalState(GameState state)
        {
            var mainCar = state.Vehicles.FirstOrDefault(v => v.IsMain);
            return mainCar != null && mainCar.X + mainCar.Length >= BOARD_COLS;
        }

        private List<GameState> GetPossibleMoves(GameState state)
        {
            var possibleMoves = new List<GameState>();
            var board = CreateBoard(state);

            for (int i = 0; i < state.Vehicles.Count; i++)
            {
                var vehicle = state.Vehicles[i];
                char vehicleSymbol = vehicle.GetDisplaySymbol();

                if (CanMove(vehicle, board, 1))
                {
                    var newState = state.Clone();
                    newState.Vehicles[i].X += vehicle.IsHorizontal ? 1 : 0;
                    newState.Vehicles[i].Y += vehicle.IsHorizontal ? 0 : 1;
                    string direction = vehicle.IsHorizontal ? "right" : "down";
                    newState.Move = $"Move {vehicleSymbol} {direction}";
                    possibleMoves.Add(newState);
                }

                if (CanMove(vehicle, board, -1))
                {
                    var newState = state.Clone();
                    newState.Vehicles[i].X += vehicle.IsHorizontal ? -1 : 0;
                    newState.Vehicles[i].Y += vehicle.IsHorizontal ? 0 : -1;
                    string direction = vehicle.IsHorizontal ? "left" : "up";
                    newState.Move = $"Move {vehicleSymbol} {direction}";
                    possibleMoves.Add(newState);
                }
            }

            return possibleMoves;
        }

        private bool CanMove(Vehicle vehicle, bool[,] board, int direction)
        {
            if (vehicle.IsObstacle) return false;
            if (vehicle.IsHorizontal)
            {
                int newX = direction > 0 ? vehicle.X + vehicle.Length : vehicle.X - 1;

                if (newX < 0 || newX >= BOARD_COLS)
                    return false;

                return !board[vehicle.Y, newX];
            }
            else
            {
                int newY = direction > 0 ? vehicle.Y + vehicle.Length : vehicle.Y - 1;

                if (newY < 0 || newY >= BOARD_ROWS)
                    return false;

                return !board[newY, vehicle.X];
            }
        }

        private bool[,] CreateBoard(GameState state)
        {
            var board = new bool[BOARD_ROWS, BOARD_COLS];

            foreach (var vehicle in state.Vehicles)
            {
                for (int i = 0; i < vehicle.Length; i++)
                {
                    if (vehicle.IsHorizontal)
                        board[vehicle.Y, vehicle.X + i] = true;
                    else
                        board[vehicle.Y + i, vehicle.X] = true;
                }
            }

            return board;
        }

        private SolutionPath CreateSolutionPath(GameState finalState)
        {
            var solution = new SolutionPath();
            var current = finalState;

            while (current != null)
            {
                if (current.Move != null)
                    solution.Moves.Add(current.Move);

                solution.BoardStates.Add(current.GetBoardState());
                current = current.Parent;
            }

            solution.Moves.Reverse();
            solution.BoardStates.Reverse();
            solution.TotalMoves = solution.Moves.Count;

            return solution;
        }

        
    }
}
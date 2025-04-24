using System;
using System.Collections.Generic;
using UnityEngine;
using static RushHourPuzzle.Graph;
using System.Linq;
using TMPro;

public class Busca : MonoBehaviour
{

    public string[,] grid = new string[10, 6];
    private HashSet<string> visited = new HashSet<string>();
    private PriorityQueue<State> priorityQueue = new PriorityQueue<State>();

    [SerializeField] private TMP_Text uiText;
    [SerializeField] private TMP_Text uiTextcopia;


    public List<State> Path { get; private set; } = new List<State>();
    public int tammax = 25;



    private class PriorityQueue<T>
    {
        private SortedDictionary<int, Queue<T>> _dict = new SortedDictionary<int, Queue<T>>();

        public void Enqueue(T item, int priority)
        {
            if (!_dict.ContainsKey(priority))
                _dict[priority] = new Queue<T>();
            _dict[priority].Enqueue(item);
        }

        public T Dequeue()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Queue is empty");
            var first = _dict.First();
            var item = first.Value.Dequeue();
            if (first.Value.Count == 0)
                _dict.Remove(first.Key);
            return item;
        }

        public bool IsEmpty()
        {
            return !_dict.Any();
        }
    }

    public void Solve(List<Vehicle> veiculos)
    {
        PreencherGrid(veiculos);
        State initialState = new State(grid);

        // Enqueue initial state with its heuristic value as priority
        priorityQueue.Enqueue(initialState, CalculateHeuristic(initialState));

        int stepsExplored = 0;
        int maxSteps = 10000; // Prevent infinite loops

        while (!priorityQueue.IsEmpty() && stepsExplored < maxSteps)
        {
            State currentState = priorityQueue.Dequeue();
            string stateKey = currentState.ToString();

            if (visited.Contains(stateKey))
                continue;

            visited.Add(stateKey);
            stepsExplored++;

            if (IsGoal(currentState))
            {
                Debug.Log($"Solu��o encontrada em {stepsExplored} passos!");
                ShowSolution(currentState);
                return;
            }

            foreach (State nextState in currentState.GetNextStates(veiculos))
            {
                if (!visited.Contains(nextState.ToString()))
                {
                    int priority = CalculateHeuristic(nextState);
                    priorityQueue.Enqueue(nextState, priority);
                }
            }
        }

        //se n encontrar solucao, manda 0
        Debug.Log("0");
    }

    private int CalculateHeuristic(State state)
    {
        // Find the red car position
        int redCarX = -1;
        int redCarY = -1;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (state.Grid[x, y] == "carro_vermelho_target")
                {
                    redCarX = x;
                    redCarY = y;
                    break;
                }
            }
            if (redCarX != -1) break;
        }

        if (redCarX == -1)
            return int.MaxValue;

        // Calculate Manhattan distance to goal position (9, 2)
        int distanceToGoal = Math.Abs(9 - redCarX) + Math.Abs(2 - redCarY);

        // Count blocking vehicles
        int blockingVehicles = 0;
        for (int x = redCarX + 1; x <= 9; x++)
        {
            if (state.Grid[x, redCarY] != "vazio")
                blockingVehicles++;
        }

        // Combined heuristic: distance + blocking vehicles * 2
        return distanceToGoal + (blockingVehicles * 2);
    }

    private State DepthLimitedSearch(State state, List<Vehicle> veiculos,
        int depthLimit, HashSet<string> visited)
    {
        if (depthLimit == 0 && IsGoal(state))
            return state;

        if (depthLimit <= 0)
            return null;

        string stateKey = state.ToString();
        if (visited.Contains(stateKey))
            return null;

        visited.Add(stateKey);

        List<State> nextStates = state.GetNextStates(veiculos);

        foreach (State nextState in nextStates)
        {
            State result = DepthLimitedSearch(nextState, veiculos, depthLimit - 1, visited);
            if (result != null)
                return result;
        }

        return null;
    }

    private void PreencherGrid(List<Vehicle> veiculos)
    {
        // Primeiro, inicialize todo o grid com espa�os vazios
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                grid[i, j] = new string("vazio");
            }
        }

        foreach (Vehicle veiculo in veiculos)
        {
            int x = veiculo.X;
            int y = veiculo.Y;

            // Verifica se as coordenadas est�o dentro dos limites
            if (x < 0 || x >= 10 || y < 0 || y >= 6)
            {
                Debug.LogError($"Coordenadas inv�lidas para ve�culo: ({x}, {y})");
                continue;
            }

            // Add veiculo a sua posicao no grid
            grid[x, y] = veiculo.Type;

            // Caso o veiculo ocupe 2 ou 3 posicoes, add no grid os outros espacos
            if (veiculo.Length == 2 || veiculo.Length == 3)
            {
                if (veiculo.IsHorizontal)
                {
                    if (x + 1 < 10) grid[x + 1, y] = veiculo.Type;
                    if (veiculo.Length == 3 && x + 2 >= 0) grid[x + 2, y] = veiculo.Type;
                }
                else
                {
                    if (y + 1 < 6) grid[x, y + 1] = veiculo.Type;
                    if (veiculo.Length == 3 && y + 2 >= 0) grid[x, y + 2] = veiculo.Type;
                }
            }
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Debug.Log($"posicao {i}-{j} = {grid[i, j]}");
            }
        }
    }

    private bool IsGoal(State state)
    {
        return state.IsCarAtGoal();
    }

    public void ShowSolution(State goalState)
    {
        if (uiText == null)
        {
            uiText = GameObject.Find("textoarthur").GetComponent<TMP_Text>();
            if (uiText == null)
            {
                Debug.LogError("Não foi possível encontrar o componente Text.");
            }
        }

        if (uiTextcopia == null)
        {
            uiTextcopia = GameObject.Find("textoarthurCopia").GetComponent<TMP_Text>();
            if (uiTextcopia == null)
            {
                Debug.LogError("Não foi possível encontrar o componente TextCopia.");
            }
        }
        List<State> path = new List<State>();
        State currentState = goalState;
        while (currentState != null)
        {
            path.Add(currentState);
            currentState = currentState.PreviousState;

            // Adiciona log de depuração se o estado anterior não for nulo
            if (currentState != null)
            {
                // Assumindo que existe um método para identificar qual carro mudou de posição
                string movedCarInfo = GetMovedCarInfo(currentState, path[path.Count - 1]);
                Debug.Log($"Carro movido: {movedCarInfo}");
            }
        }
        path.Reverse();

        if (uiText == null)
        {
            Debug.LogError("uiText não está atribuído!");
            return;
        }

        System.Random random = new System.Random();
        int randomIntInRange = random.Next(2, 7);

        tammax = path.Count + randomIntInRange;
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>(); // Ou use GetComponent se for o mesmo GameObject
        if (gameOverManager != null)
        {
            gameOverManager.AtualizarTammax(tammax);  // Passando o valor de tammax
        }
        Debug.Log("tammax valor : " + tammax);
        uiText.text = $"{path.Count - 1 + randomIntInRange} passos";
        uiTextcopia.text = $"Max: {path.Count - 1 + randomIntInRange}";
    }


    // M�todo auxiliar para determinar qual carro mudou e para onde
    private string GetMovedCarInfo(State previousState, State currentState)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (previousState.Grid[i, j] != currentState.Grid[i, j])
                {
                    return $"Posicao {i}-{j} antigo = {previousState.Grid[i, j]} para = {currentState.Grid[i, j]}";
                }
            }
        }
        Debug.Log("---------------");
        return "Nenhum carro movido";
    }

    /*private void PrintGrid(string[,] grid)
    {
        string gridString = "\n";
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                gridString += grid[x, y].name.PadRight(15) + " ";
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
    }*/
}

public class State
{
    public string[,] Grid { get; private set; }
    public State PreviousState { get; private set; }
    private Vector2Int goalPosition = new Vector2Int(9, 3);

    public State(String[,] grid, State previousState = null)
    {
        Grid = new string[grid.GetLength(0), grid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Grid[i, j] = grid[i, j];
            }
        }
        PreviousState = previousState;
    }

    public List<State> GetNextStates(List<Vehicle> v)
    {
        List<State> nextStates = new List<State>();
        Dictionary<string, VehicleInfo> processedVehicles = new Dictionary<string, VehicleInfo>();

        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                string currentCar = Grid[x, y];
                if (currentCar != "vazio" && currentCar != "obs1" && currentCar != "obs2" && currentCar != "obs3" && !processedVehicles.ContainsKey(currentCar))
                {
                    VehicleInfo vehicleInfo = new VehicleInfo
                    {
                        s = currentCar,
                        X = x,
                        Y = y,
                        IsHorizontal = DetermineOrientation(v, x, y),
                        Length = DetermineLength(v, x, y)
                    };
                    //Debug.Log($"X= {vehicleInfo.X}, Y= {vehicleInfo.Y}, Length= {vehicleInfo.Length}, Horizontal= {vehicleInfo.IsHorizontal}");
                    processedVehicles[currentCar] = vehicleInfo;

                    if (vehicleInfo.IsHorizontal)
                    {
                        // Tenta mover horizontalmente
                        if (CanMove(x, y, 1, 0, vehicleInfo))
                            nextStates.Add(CreateNewState(x, y, vehicleInfo, 1, 0));
                        if (CanMove(x, y, -1, 0, vehicleInfo))
                            nextStates.Add(CreateNewState(x, y, vehicleInfo, -1, 0));
                    }
                    else
                    {
                        // Tenta mover verticalmente
                        if (CanMove(x, y, 0, 1, vehicleInfo))
                            nextStates.Add(CreateNewState(x, y, vehicleInfo, 0, 1));
                        if (CanMove(x, y, 0, -1, vehicleInfo))
                            nextStates.Add(CreateNewState(x, y, vehicleInfo, 0, -1));
                    }
                }
            }
        }

        return nextStates;
    }

    private class VehicleInfo
    {
        public string s { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsHorizontal { get; set; }
        public int Length { get; set; }
    }

    private bool DetermineOrientation(List<Vehicle> veiculos, int x, int y)
    {
        string v2Name = Grid[x, y];

        // Procurar o GameObject pelo nome
        GameObject v2 = GameObject.Find(v2Name);

        if (v2 == null)
        {
            Debug.LogError($"GameObject com o nome {v2Name} n�o encontrado.");
            return false;
        }

        // Encontrar o ve�culo correspondente na lista de ve�culos
        Vehicle veiculoEncontrado = veiculos.FirstOrDefault(veiculo => veiculo.Type == v2Name);

        if (veiculoEncontrado == null)
        {
            Debug.LogError("Ve�culo n�o encontrado na posi��o especificada.");
            return false;
        }

        return veiculoEncontrado.IsHorizontal;
    }

    private int DetermineLength(List<Vehicle> veiculos, int x, int y)
    {
        string v2Name = Grid[x, y];

        // Procurar o GameObject pelo nome
        GameObject v2 = GameObject.Find(v2Name);

        if (v2 == null)
        {
            Debug.LogError($"GameObject com o nome {v2Name} n�o encontrado.");
            return 0;
        }

        // Encontrar o ve�culo correspondente na lista de ve�culos
        Vehicle veiculoEncontrado = veiculos.FirstOrDefault(veiculo => veiculo.Type == v2Name);

        if (veiculoEncontrado == null)
        {
            Debug.LogError("Ve�culo n�o encontrado na posi��o especificada.");
            return 0;
        }

        return veiculoEncontrado.Length;
    }

    private bool CanMove(int x, int y, int dx, int dy, VehicleInfo vehicle)
    {
        int newX = x;
        int newY = y;

        // Verifica se a nova posi��o est� dentro dos limites
        if (vehicle.IsHorizontal)
        {
            if (dx > 0 && (x + vehicle.Length >= Grid.GetLength(0))) return false;
            if (dx < 0 && (x <= 0)) return false;
            //newX = x + dx;
            newX = dx > 0 ? x + vehicle.Length : x - 1;
        }
        else
        {
            if (dy > 0 && (y + vehicle.Length >= Grid.GetLength(1))) return false;
            if (dy < 0 && (y <= 0)) return false;
            //newY = y + dy;
            newY = dy > 0 ? y + vehicle.Length : y - 1;
        }

        // Verifica se a nova posi��o est� ocupada
        return newX >= 0 && newX < Grid.GetLength(0) &&
               newY >= 0 && newY < Grid.GetLength(1) &&
               Grid[newX, newY] == "vazio";
    }

    private State CreateNewState(int x, int y, VehicleInfo vehicle, int dx, int dy)
    {
        State newState = new State(Grid, this);
        string vehicleName = vehicle.s;

        // Remove o ve�culo da posi��o atual
        for (int i = 0; i < vehicle.Length; i++)
        {
            int currentX = x + (vehicle.IsHorizontal ? i : 0);
            int currentY = y + (vehicle.IsHorizontal ? 0 : i);
            newState.Grid[currentX, currentY] = new string("vazio");
        }

        // Coloca o ve�culo na nova posi��o
        for (int i = 0; i < vehicle.Length; i++)
        {
            int newX = x + dx + (vehicle.IsHorizontal ? i : 0);
            int newY = y + dy + (vehicle.IsHorizontal ? 0 : i);
            newState.Grid[newX, newY] = vehicle.s;
        }

        return newState;
    }

    public bool IsCarAtGoal()
    {
        return Grid[9, 3] == "carro_vermelho_target";
    }

    public override string ToString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                sb.Append(Grid[x, y]);
            }
        }
        return sb.ToString();
    }
}

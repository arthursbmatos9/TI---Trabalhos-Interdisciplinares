using GerarMapasNamespace;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static RushHourPuzzle.Graph;
namespace GerarMapasNamespace
{
    public class GerarMapaAleatorio : MonoBehaviour
    {
        // Atributos
        public GameObject asfalto_estacionamento;
        public GameObject veiculos;
        public GameObject carroVermelhoTarget;
        private bool[,] ocupados = new bool[10, 6];



        // Lista que cont�m todos os objetos do tipo Vehicle
        public List<Vehicle> veiculos_mapeados = new List<Vehicle>();
        private string tempFolderPath;
        private string initialStatePath;
        private string solutionPath;

        // Model Ve�culos

        void Awake()
        {
            // Cria o diretório temporário se não existir
            tempFolderPath = Path.Combine(Application.temporaryCachePath, "RushHourTemp");
            initialStatePath = Path.Combine(tempFolderPath, "initial_state.dat");
            solutionPath = Path.Combine(tempFolderPath, "solution.dat");

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
        }



        public List<Vehicle> pegaListaVeiculos()
        {
            if (veiculos_mapeados == null || veiculos_mapeados.Count == 0)
            {
                Debug.LogError("Lista de ve�culos vazia ou n�o inicializada!");
                return new List<Vehicle>();  // Retorna uma lista vazia em caso de erro
            }
            return veiculos_mapeados;
        }

        [System.Serializable]
        public class SerializableVehicle
        {
            public int X;
            public int Y;
            public bool IsHorizontal;
            public int Length;
            public string Type;
            public bool IsMain;
            public bool IsObstacle;
            // Adicionando campos para posição e rotação
            public Vector3 Position;
            public Quaternion Rotation;

            public SerializableVehicle(Vehicle v, GameObject vehicleObject)
            {
                X = v.X;
                Y = v.Y;
                IsHorizontal = v.IsHorizontal;
                Length = v.Length;
                Type = v.Type;
                IsMain = v.IsMain;
                IsObstacle = v.IsObstacle;
                // Salvando posição e rotação do GameObject
                Position = vehicleObject.transform.position;
                Rotation = vehicleObject.transform.rotation;
            }
        }
        [System.Serializable]
        public class SerializableSolution
        {
            public List<string> Moves;
            public int TotalMoves;

            public SerializableSolution(RushHourPuzzle.Graph.SolutionPath solution)
            {
                Moves = solution.Moves;
                TotalMoves = solution.TotalMoves;
            }
        }

        private void SaveInitialState(List<Vehicle> vehicles)
        {
            try
            {
                var serializableVehicles = vehicles.Select(v =>
                {
                    GameObject vehicleObject = veiculos.transform.Find(v.Type).gameObject;
                    return new SerializableVehicle(v, vehicleObject);
                }).ToList();

                var wrapper = new SerializableVehicleWrapper { vehicles = serializableVehicles };
                string json = JsonUtility.ToJson(wrapper);
                File.WriteAllText(initialStatePath, json);
                Debug.Log($"Estado inicial salvo em: {initialStatePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao salvar estado inicial: {e.Message}");
            }
        }




        private void SaveSolution(RushHourPuzzle.Graph.SolutionPath solution)
        {
            try
            {
                var serializableSolution = new SerializableSolution(solution);
                string json = JsonUtility.ToJson(serializableSolution);
                File.WriteAllText(solutionPath, json);
                Debug.Log($"Solução salva em: {solutionPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao salvar solução: {e.Message}");
            }
        }


        public void gerarMapa()
        {
            int gerouCerto = 0;
            while (gerouCerto < 10) // adiciona mais 3 segundos pra carregar o mapa, pra ser mais rapido usar bool ao inves de int
            {
                veiculos_mapeados.Clear();
                ocupados = new bool[10, 6];
                //  Atributos
                float largura = asfalto_estacionamento.GetComponent<Renderer>().bounds.size.x / 10;
                float altura = asfalto_estacionamento.GetComponent<Renderer>().bounds.size.y / 6;
                Vector2[,] grid = new Vector2[10, 6];
                Vector3 origin = asfalto_estacionamento.transform.position - new Vector3(asfalto_estacionamento.GetComponent<Renderer>().bounds.extents.x,
                                                                                          asfalto_estacionamento.GetComponent<Renderer>().bounds.extents.y, 0);

                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 6; y++)
                    {
                        grid[x, y] = new Vector2(origin.x + (x + 0.5f) * largura, origin.y + (y + 0.5f) * altura);
                        ocupados[x, y] = false;
                    }

                // Definir a posi��o do carro vermelho
                carroVermelhoTarget.transform.position = new Vector3(grid[0, 3].x, grid[0, 3].y, carroVermelhoTarget.transform.position.z);
                carroVermelhoTarget.transform.rotation = Quaternion.Euler(0, 0, -90);
                ocupados[0, 3] = true;

                // Adiciona o ve�culo principal (carro vermelho) � lista
                Vehicle v = new Vehicle
                (
                    0,
                    3,
                    true,
                    1,
                    "carro_vermelho_target",
                    true,
                    false
                );
                veiculos_mapeados.Add(v);

                // Posicionando outros ve�culos
                foreach (Transform veiculoTransform in veiculos.transform)
                {
                    GameObject Vehicle = veiculoTransform.gameObject;

                    // Ignora o carro vermelho, pois j� foi posicionado
                    if (Vehicle.name == "carro_vermelho_target") continue;

                    int tamanho = Vehicle.name.Contains("bus") ? 3 : Vehicle.name.Contains("truck") ? 2 : 1;
                    bool posicaoValida = false;
                    int posX, posY;
                    bool horizontal;

                    do
                    {
                        // Para ve�culos com comprimento 1, 2 ou 3
                        if (Vehicle.name.Contains("bus") || Vehicle.name.Contains("truck"))
                        {
                            posX = Random.Range(1, 9);  // Garantindo que n�o fique nas bordas
                            posY = Random.Range(1, 5);  // Garantindo que n�o fique nas bordas
                        }
                        else if (Vehicle.name.Contains("obs"))
                        {
                            // Obst�culos n�o podem estar na linha 3 (onde o carro vermelho est�)
                            do
                            {
                                posX = Random.Range(0, 10);
                                posY = Random.Range(0, 6);
                            } while (posY == 3); // Linha 3 (onde o carro vermelho est�)
                        }
                        else
                        {
                            posX = Random.Range(0, 10);
                            posY = Random.Range(0, 6);
                        }

                        if (posY == 3)
                            horizontal = true; // For�a vertical se estiver na linha 3 (para nao ficar na horizontal na linha do vermelho)
                        else
                            horizontal = Random.Range(0, 2) == 0;

                        posicaoValida = VerificarPosicaoValida(posX, posY, tamanho, horizontal);
                    } while (!posicaoValida);



                    // Posiciona o ve�culo
                    if (Vehicle.name.Contains("truck"))
                    {
                        if (horizontal)
                        {
                            // Truck horizontal (na verdade, vertical)
                            float meioY = (grid[posX, posY].y + grid[posX, posY + 1].y) / 2;
                            Vehicle.transform.position = new Vector3(grid[posX, posY].x, meioY, Vehicle.transform.position.z);
                        }
                        else
                        {
                            // Truck vertical (na verdade, horizontal)
                            float meioX = (grid[posX, posY].x + grid[posX + 1, posY].x) / 2;
                            Vehicle.transform.position = new Vector3(meioX, grid[posX, posY].y, Vehicle.transform.position.z);
                        }
                    }
                    else
                    {
                        Vehicle.transform.position = new Vector3(grid[posX, posY].x, grid[posX, posY].y, Vehicle.transform.position.z);
                    }

                    // Rota��o do ve�culo
                    Vehicle.transform.rotation = horizontal ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 90);

                    // Atualiza posi��es ocupadas
                    MarcarPosicoesOcupadas(posX, posY, tamanho, horizontal);

                    // Adiciona o ve�culo � lista de mapeados
                    v = new Vehicle
                    (
                        posX,
                        posY,
                        !horizontal,
                        tamanho,
                        Vehicle.name,
                        Vehicle.name.Contains("obs"),
                        false
                    );
                    if (v.Length == 3)
                    {
                        if (v.IsHorizontal)
                        {
                            v.X--;
                        }
                        else
                        {
                            v.Y--;
                        }
                    }
                    //else if (v.Length == 2) {
                    //    if (!v.IsHorizontal)
                    //    {
                    //        v.Y++;
                    //    }
                    //}
                    veiculos_mapeados.Add(v);
                    for (int i = 0; i < veiculos_mapeados.Count; i++)
                    {
                        Vehicle veiculo1 = veiculos_mapeados[i];
                        Debug.Log($"Veículo: {veiculo1.Type}, Posição: ({veiculo1.X}, {veiculo1.Y}), Horizontal: {veiculo1.IsHorizontal}, Comprimento: {veiculo1.Length}, É Principal: {veiculo1.IsMain}, É Obstáculo: {veiculo1.IsObstacle}");
                    }
                }

                // Imprime a lista de ve�culos mapeados
                ImprimirVeiculos();
                RushHourPuzzle rushHourPuzzle = new RushHourPuzzle();
                gerouCerto = rushHourPuzzle.SolvePuzzle(veiculos_mapeados);
                if (gerouCerto > 0)
                {
                    // Salva o estado inicial
                    SaveInitialState(veiculos_mapeados);

                    // Obtém e salva a solução
                    var solution = rushHourPuzzle.GetLastSolution(); // Você precisará adicionar este método em RushHourPuzzle
                    if (solution != null)
                    {
                        SaveSolution(solution);
                    }
                }
            }
            Busca busca = new Busca();
            busca.Solve(veiculos_mapeados);
        }

        // Método para carregar o estado inicial (útil para reset)
        public List<Vehicle> LoadInitialState()
        {
            try
            {
                if (File.Exists(initialStatePath))
                {
                    string json = File.ReadAllText(initialStatePath);
                    var wrapper = JsonUtility.FromJson<SerializableVehicleWrapper>(json);
                    return wrapper.vehicles.Select(sv => new Vehicle(
                        sv.X, sv.Y, sv.IsHorizontal, sv.Length, sv.Type, sv.IsMain, sv.IsObstacle
                    )).ToList();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao carregar estado inicial: {e.Message}");
            }
            return null;
        }

        public RushHourPuzzle.Graph.SolutionPath LoadSolution()
        {
            try
            {
                if (File.Exists(solutionPath))
                {
                    string json = File.ReadAllText(solutionPath);
                    var serializableSolution = JsonUtility.FromJson<SerializableSolution>(json);
                    // Você precisará criar um construtor apropriado em SolutionPath
                    return new RushHourPuzzle.Graph.SolutionPath(serializableSolution.Moves, serializableSolution.TotalMoves);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao carregar solução: {e.Message}");
            }
            return null;
        }

        private void ImprimirVeiculos()
        {
            Debug.Log("Lista de veículos mapeados:");

            // Dimensões da matriz (ajuste conforme necessário)
            int linhas = 10;
            int colunas = 6;
            char[,] mapa = new char[linhas, colunas];

            // Inicializa a matriz com espaços vazios
            for (int i = 0; i < linhas; i++)
            {
                for (int j = 0; j < colunas; j++)
                {
                    mapa[i, j] = '.'; // Representa uma célula vazia
                }
            }

            // Preenche a matriz com os veículos
            foreach (Vehicle Vehicle in veiculos_mapeados)
            {
                //Debug.Log($"Veículo: {Vehicle.Nome}, Posição: ({Vehicle.X}, {Vehicle.Y}), Horizontal: {Vehicle.IsHorizontal}, Comprimento: {Vehicle.Length}, É Principal: {Vehicle.IsMain}, É Obstáculo: {Vehicle.IsObstacle}");

                // Determina o caractere a ser usado para o veículo
                char simbolo = Vehicle.IsMain ? 'P' : (Vehicle.IsObstacle ? 'O' : 'V');

                // Preenche a matriz com base na posição e orientação do veículo
                for (int i = 0; i < Vehicle.Length; i++)
                {
                    int x = Vehicle.X;
                    int y = Vehicle.Y;

                    if (Vehicle.IsHorizontal)
                    {
                        x += i; // Incrementa a coluna para veículos horizontais
                    }
                    else
                    {
                        y += i; // Incrementa a linha para veículos verticais
                    }

                    // Verifica se a posição está dentro dos limites
                    if (x >= 0 && x < linhas && y >= 0 && y < colunas)
                    {
                        mapa[x, y] = simbolo;
                    }
                    else
                    {
                        Debug.LogWarning($"Veículo {Vehicle.Type} está fora dos limites do mapa!");
                    }
                }
            }

            // Imprime a matriz no console
            Debug.Log("Mapa de veículos:");
            string temp = "";
            for (int i = 0; i < linhas; i++)
            {
                string linha = "";
                for (int j = 0; j < colunas; j++)
                {
                    linha += mapa[i, j] + " ";
                }
                temp += linha + "\n";
            }
            Debug.Log(temp);
        }


        private bool VerificarPosicaoValida(int posX, int posY, int tamanho, bool horizontal)
        {
            // Se a posi��o j� estiver ocupada
            if (ocupados[posX, posY]) return false;

            // Se o ve�culo for horizontal (ocupando duas colunas), verificar linha 3
            if (tamanho == 2)
            {
                if (horizontal) // Ve�culo horizontal (na verdade, vertical)
                {
                    // Verifica a linha 3 para ambas as colunas (posY e posY+1)
                    if (posY == 3 || ocupados[posX, posY + 1])
                        return false;
                }
                else // horizontal
                {
                    if (ocupados[posX + 1, posY])
                        return false;
                }
            }

            // Para ve�culos com comprimento 3
            if (tamanho == 3)
            {
                if (horizontal) // Horizontal (na verdade, vertical)
                {
                    // Verifica a linha 3 para as duas colunas
                    if (posY == 3 || ocupados[posX, posY + 1] || ocupados[posX, posY - 1])
                        return false;
                }
                else // horizontal
                {
                    if (ocupados[posX + 1, posY] || ocupados[posX - 1, posY])
                        return false;
                }
            }

            return true;
        }



        private void MarcarPosicoesOcupadas(int posX, int posY, int tamanho, bool horizontal)
        {
            ocupados[posX, posY] = true;

            if (tamanho == 2)
            {
                if (horizontal)
                    ocupados[posX, posY + 1] = true;
                else
                    ocupados[posX + 1, posY] = true;
            }

            if (tamanho == 3)
            {
                if (horizontal)
                {
                    ocupados[posX, posY + 1] = true;
                    ocupados[posX, posY - 1] = true;
                }
                else
                {
                    ocupados[posX + 1, posY] = true;
                    ocupados[posX - 1, posY] = true;
                }
            }
        }
    }
}

[System.Serializable]
public class SerializableVehicleWrapper
{
    public List<GerarMapaAleatorio.SerializableVehicle> vehicles;
}

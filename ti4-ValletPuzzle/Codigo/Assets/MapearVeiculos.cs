using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapearVeiculos : MonoBehaviour
{
    public GameObject asfalto_estacionamento;
    public GameObject veiculos; // GameObject pai que contém todos os veículos

    public void MapearVeiculosBotao()
    {
        Vector2[,] grid = new Vector2[10, 6];
        float largura = asfalto_estacionamento.GetComponent<Renderer>().bounds.size.x / 10;
        float altura = asfalto_estacionamento.GetComponent<Renderer>().bounds.size.y / 6;

        Vector3 origin = asfalto_estacionamento.transform.position - new Vector3(asfalto_estacionamento.GetComponent<Renderer>().bounds.extents.x,
                                                                                  asfalto_estacionamento.GetComponent<Renderer>().bounds.extents.y, 0);

        // Criando a matriz
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 6; y++)
            {
                grid[x, y] = new Vector2(origin.x + x * largura, origin.y + y * altura); // Ajuste no cálculo do Y
            }
        }

        // Mapeando as posições dos veículos
        foreach (Transform veiculoTransform in veiculos.transform)
        {
            GameObject veiculo = veiculoTransform.gameObject;
            Renderer renderer = veiculo.GetComponent<Renderer>();
            Vector3 posVeiculo = veiculo.transform.position;

            Veiculo v = new Veiculo();
            v.Type = veiculo;            //identificador do veiculo (seu nome, q eh unico)

            //verificando se eh o vermelho ou objeto
            if(veiculo.name == "carro_vermelho_target") v.IsMain = true;
            else v.IsMain = false;
            if (veiculo.name == "obs1" || veiculo.name == "obs2" || veiculo.name == "obs3") v.IsObstacle = true;
            else v.IsObstacle = false;

            // Verifica se o veículo está na horizontal ou vertical
            bool isHorizontal = renderer.bounds.size.x > renderer.bounds.size.y;
            int tamanho = Mathf.RoundToInt(isHorizontal ? renderer.bounds.size.x / largura : renderer.bounds.size.y / altura);
            v.IsHorizontal = isHorizontal; 
            v.Length = tamanho;

            // Calcula a menor posição na matriz
            int cellX = Mathf.Clamp((int)((posVeiculo.x - origin.x) / largura), 0, 9);
            int cellY = (int)((posVeiculo.y - origin.y) / altura);
            cellY = 5 - cellY; // Inverte o eixo Y para começar de cima para baixo
            v.X = cellX;
            v.Y = cellY;

            if(veiculo.name == "bus1" || veiculo.name == "bus2" || veiculo.name == "bus3")
            {
                if (isHorizontal)
                    cellX--;
                else
                    cellY--;
            }
            if (veiculo.name == "truck_marrom" || veiculo.name == "truck_azul_claro" || veiculo.name == "truck_azul_escuro" || veiculo.name == "truck_vermelho")
            {
                if (isHorizontal)
                    cellX--;
            }
            if(veiculo.name == "truck_preto")
            {
                if (!isHorizontal)
                    cellY--;
            }

            Debug.Log($"Veículo {veiculo.name} está na {(isHorizontal ? "horizontal" : "vertical")} e ocupa {tamanho} posições.");
            Debug.Log($"Posição inicial na matriz: ({cellX}, {cellY})");

            // Mapeia todas as posições ocupadas pelo veículo
            for (int i = 0; i < tamanho; i++)
            {
                int x = isHorizontal ? cellX + i : cellX;
                int y = isHorizontal ? cellY : cellY + i;

                if (x < 10 && y < 6) // Verifica se está dentro dos limites da matriz
                {
                    Debug.Log($"Veículo {veiculo.name} ocupa a posição ({x}, {y})");
                }
            }
        }
    }
}

public class Veiculo
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsHorizontal { get; set; }
    public int Length { get; set; }
    public bool IsMain { get; set; }
    public bool IsObstacle { get; set; }
    public GameObject Type { get; set; }
}

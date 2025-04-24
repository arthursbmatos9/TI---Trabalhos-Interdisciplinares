using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public int tammaxRecebido; // Limite máximo de movimentos
    public GameObject popupPanel3; // Popup de Game Over

    public VehicleSelector vehicleSelector; // Referência para o VehicleSelector

     private bool popupShown = false; // Controle para mostrar o popup apenas uma vez

    private void Start()
    {
        // Atualiza o valor de tammaxRecebido ao iniciar o jogo
        AtualizarTammax(tammaxRecebido);

        // Garante que o vehicleSelector seja atribuído
        if (vehicleSelector == null)
        {
            vehicleSelector = FindObjectOfType<VehicleSelector>();
        }

        
    }

    private void Update(){
        // Verifica o Game Over imediatamente ao começar
        VerificarGameOver();
    }

    // Método para atualizar o limite máximo de movimentos
    public void AtualizarTammax(int novoTammax)
    {
        tammaxRecebido = novoTammax-1;
        Debug.Log("Valor de tammax recebido: " + tammaxRecebido);
    }

    // Método para verificar se o Game Over ocorreu
    public void VerificarGameOver()
    {
        if(tammaxRecebido==-1){
            tammaxRecebido=25;
        }
        if (vehicleSelector != null && vehicleSelector.moveCount > tammaxRecebido)
        {
            ShowGameOverPopup();
        }
         Debug.Log($"Movimentos realizados: {vehicleSelector.moveCount}, Limite máximo: {tammaxRecebido}");
    }

    // Método para mostrar o popup de Game Over
    void ShowGameOverPopup()
    {
        if (popupPanel3 != null)
        {
            popupPanel3.SetActive(true); // Ativa o painel de Game Over
            Debug.Log($"Game Over! Movimentos realizados: {vehicleSelector.moveCount}, Limite máximo: {tammaxRecebido}");
        }
        else
        {
            Debug.LogWarning("Popup Panel 3 (Game Over) não atribuído!");
        }
    }
}




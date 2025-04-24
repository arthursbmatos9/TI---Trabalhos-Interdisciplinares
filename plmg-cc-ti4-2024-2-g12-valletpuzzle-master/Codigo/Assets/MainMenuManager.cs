using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;

    [SerializeField] private string restart;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelConfig;
    [SerializeField] private GameObject painelComo;
    [SerializeField] private GameObject painelCreditos;

    public void Jogar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void Restart()
    {
        SceneManager.LoadScene(restart);
    }

    public void AbrirConfig()
    {
        painelMenuInicial.SetActive(false);
        painelConfig.SetActive(true);
    }

    public void FecharConfig()
    {
        painelConfig.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

        public void AbrirComo()
    {
        painelMenuInicial.SetActive(false);
        painelComo.SetActive(true);
    }

    public void FecharComo()
    {
        painelComo.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

            public void AbrirCreditos()
    {
        painelMenuInicial.SetActive(false);
        painelCreditos.SetActive(true);
    }

    public void FecharCReditos()
    {
        painelCreditos.SetActive(false);
        painelMenuInicial.SetActive(true);
    }


    public void Sair()
    {
        Debug.Log("Sair do jogo");
        Application.Quit();
    }
}

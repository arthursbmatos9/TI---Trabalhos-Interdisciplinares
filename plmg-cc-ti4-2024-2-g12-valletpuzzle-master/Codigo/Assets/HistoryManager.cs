using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Adicione este namespace
using UnityEngine.SceneManagement;

public class HistoryManager : MonoBehaviour
{
    [SerializeField] private string nomeJogo;
    [SerializeField] private string nomeMenu;
    public void Jogar()
    {
        SceneManager.LoadScene(nomeJogo);
    }

    public void Menu()
    {
        SceneManager.LoadScene(nomeMenu);
    }

}

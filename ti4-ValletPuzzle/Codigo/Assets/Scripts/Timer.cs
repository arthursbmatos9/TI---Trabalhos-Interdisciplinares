using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timeLevelTxt; // texto do botão tem q ser texmeshproUI
    private float timeLevel;
    private bool pause;

   // [SerializeField] private GameObject painel_lateral;
    [SerializeField] private GameObject painelpausado;

    [SerializeField] private GameObject painelLateral;

    public VehicleSelector VehicleSelector;

    // Start is called before the first frame update
    void Start()
    {
        pause = false; // Inicia sem pausa
        Debug.Log("timeLevelTxt assigned: " + (timeLevelTxt != null));
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause) // Se não estiver pausado
        {
            timeLevel += Time.deltaTime; // Atualiza o tempo

            // Calcula os minutos e segundos
            int minutes = Mathf.FloorToInt(timeLevel / 60F);
            int seconds = Mathf.FloorToInt(timeLevel % 60F);

            // Verifica se o timeLevelTxt não é nulo antes de tentar acessar
            if (timeLevelTxt != null)
            {
                // Exibe o formato de minutos e segundos no texto do botão
                timeLevelTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                Debug.LogWarning("timeLevelTxt is null, please assign it in the Inspector.");
            }
        }
    }

    public void AbrirPause() {
        pause = true;
        painelpausado.SetActive(true);
        painelLateral.SetActive(false);
        VehicleSelector.enabled = false;
    }
    
    public void FecharPause()
    {
        pause = false;
        painelpausado.SetActive(false);
        painelLateral.SetActive(true);
        VehicleSelector.enabled = true;
    }

    
}

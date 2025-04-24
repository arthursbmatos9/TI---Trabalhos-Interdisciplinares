using UnityEngine;

public class CheckPosition : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(5.477f, 0.497f, 0f); // Posição alvo
    public GameObject popupPanel;   // Referência ao painel padrão
    public GameObject popupPanel2;  // Referência ao painel alternativo

    public GameObject popupPanel3;
    public VehicleSelector vehicleSelector; // Referência ao script VehicleSelector

    private bool popupShown = false; // Controle para mostrar o popup apenas uma vez

    void Start()
    {
        // Se a referência não foi atribuída no Inspector, tentar encontrar automaticamente
        if (vehicleSelector == null)
        {
            vehicleSelector = FindObjectOfType<VehicleSelector>();
        }

        if (vehicleSelector == null)
        {
            Debug.LogError("VehicleSelector não foi encontrado!");
        }
    }

    void Update()
    {
        // Verifica se o carro está próximo da posição alvo
        if (!popupShown && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ShowPopupBasedOnMoveCount();
        }
    }

    void ShowPopupBasedOnMoveCount()
    {
        //popupShown = true; // Garante que o pop-up aparece apenas uma vez

        if (vehicleSelector != null)
        {
            if (vehicleSelector.moveCount == 0)
            {
                ActivatePopup(popupPanel2); // Chama o painel alternativo
            }
            else
            {
                ActivatePopup(popupPanel); // Chama o painel padrão
            }
        }
        else
        {
            Debug.LogError("VehicleSelector não atribuído!");
        }
    }

    void ActivatePopup(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true); // Ativa o painel passado como argumento
        }
        else
        {
            Debug.LogWarning("Painel não atribuído!");
        }
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false); // Desativa o painel padrão
        }
        if (popupPanel2 != null)
        {
            popupPanel2.SetActive(false); // Desativa o painel alternativo
        }
        if (popupPanel3 != null)
        {
            popupPanel3.SetActive(false); // Desativa o painel alternativo
        }

        popupShown = false; // Permite que o popup seja mostrado novamente, se necessário
        Debug.Log("Pop-up fechado");
    }
}




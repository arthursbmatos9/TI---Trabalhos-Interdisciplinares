using UnityEngine;
using UnityEngine.UI; // Para acessar botões, se necessário

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel; // Referência ao painel (popup)
   // public Button triggerButton; // Botão que ativa o painel (se necessário)

    private void Start()
    {
        // O painel começa visível, você pode desmarcar isso caso não queira.
        popupPanel.SetActive(true);

     /*   // Adicionando a ação ao botão, se houver um botão
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(ShowPopupFor5Seconds);
        }*/

        // Desativa o painel depois de 5 segundos ao iniciar
        Invoke("HidePopup", 4f);
    }

    // Função para ativar o painel por 5 segundos
    public void ShowPopupFor5Seconds()
    {
        popupPanel.SetActive(true);
        Invoke("HidePopup", 4f); // Chama a função HidePopup após 5 segundos
    }

    // Função para esconder o painel
    private void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}


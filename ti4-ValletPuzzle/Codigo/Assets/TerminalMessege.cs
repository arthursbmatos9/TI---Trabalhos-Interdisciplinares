using UnityEngine;
using UnityEngine.UI;  // Use UnityEngine.TextMeshPro para TextMeshPro
using TMPro;
public class TerminalMessege : MonoBehaviour
{
    public TextMeshProUGUI terminalText;  // Arraste o componente de texto no Inspector

    public void DisplayMessage(string message)
    {
        terminalText.text += message + "\n";  // Adiciona a nova mensagem na linha de baixo
    }

    public void ClearTerminal()
    {
        terminalText.text = "";  // Limpa o "terminal"
    }
}
using UnityEngine;
using UnityEngine.UI; // Para trabalhar com UI (Slider)

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource; // Fonte de áudio que tocará a música

    [SerializeField] private Slider volumeSlider; // Referência ao Slider na cena

    private void Awake()
    {
        // Obtém o componente AudioSource ao iniciar
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Define um volume padrão caso necessário
        float defaultVolume = 0.01f;
        audioSource.volume = defaultVolume;

        // Configura o Slider de volume, caso ele esteja atribuído
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume; // Inicializa o valor do Slider com o volume atual
            volumeSlider.onValueChanged.AddListener(ChangeVolume); // Conecta o Slider ao método para alterar o volume
        }
    }

    // Método chamado quando o valor do Slider é alterado
    public void ChangeVolume(float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value; // Atualiza o volume do AudioSource
        }
    }
}




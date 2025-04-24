using UnityEngine;
using TMPro;

public class TextAnimation2 : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Referência ao texto
    public float blinkSpeed = 1f; // Velocidade do piscar

    private bool isVisible = true;

    void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Oscilar alpha (transparência) entre 0 e 1
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro; // Adicione esta linha

public class TypeTextAnimation : MonoBehaviour
{
    public Action TypeFinished;

    public float typeDelay = 0.03f;
    public TextMeshProUGUI textObject;
    public string fullText;

    Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartTyping(){
        coroutine=StartCoroutine(TypeText());
    }

    IEnumerator TypeText(){
        textObject.text= fullText;
        textObject.maxVisibleCharacters=0;
        for(int i=0; i< textObject.text.Length; i++){
            textObject.maxVisibleCharacters=i;
            yield return new WaitForSeconds(typeDelay);
        }
        TypeFinished?.Invoke();
    }

    public void Skip(){
        StopCoroutine(coroutine);
        textObject.maxVisibleCharacters = textObject.text.Length;
    }
}

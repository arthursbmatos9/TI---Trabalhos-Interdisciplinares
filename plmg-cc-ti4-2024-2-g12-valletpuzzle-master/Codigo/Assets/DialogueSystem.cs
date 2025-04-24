using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public enum STATE
{
    DISABLED,
    WAITING,
    TYPING
}

public class DialogueSystem : MonoBehaviour
{
    public DialogueData dialogueData;

    int currentText = 0;
    bool finished = false;

    public GameObject nextSceneButton;
    public GameObject MenuSceneButton; // Arraste o botão no Unity Inspector


    TypeTextAnimation typeText;

    STATE state;

    void Awake()
    {
        typeText = FindObjectOfType<TypeTextAnimation>(); // Corrigido para usar o nome correto da classe
        typeText.TypeFinished = OnTypeFinished;
    }

    // Start is called before the first frame update
    void Start()
    {
        state = STATE.WAITING; // Define o estado inicial como WAITING
        Next(); // Inicia o diálogo automaticamente
    }


    // Update is called once per frame
    void Update()
    {
        if (state == STATE.DISABLED) return; // Corrigido para comparar valores do enum corretamente

        switch (state)
        {
            case STATE.WAITING:
                Waiting();
                break;
            case STATE.TYPING:
                Typing();
                break;
        }
    }

    public void Next()
    {
        typeText.fullText = dialogueData.talkScript[currentText++].text;

        if (currentText == dialogueData.talkScript.Count)
        {
            finished = true;
            Debug.Log("Dialog finished, setting finished to true.");
        }

        typeText.StartTyping();
        state = STATE.TYPING;
    }


    void OnTypeFinished()
    {
        state = STATE.WAITING;
    }

    void Waiting()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!finished)
            {
                Next();
            }
            else
            {
                state = STATE.DISABLED;
                currentText = 0;
                finished = false;
                Debug.Log("Dialog Finished, enabling button.");
                nextSceneButton.SetActive(true); // Habilita o botão
                nextSceneButton.GetComponent<Button>().interactable = true;
                MenuSceneButton.SetActive(true); // Habilita o botão
                MenuSceneButton.GetComponent<Button>().interactable = true; // Garante que o botão está clicável

            }
        }
    }


    void Typing()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            typeText.Skip();
            state = STATE.WAITING;
        }
    }
}
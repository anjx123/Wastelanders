using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GameObject dialogueBoxObj;
    private DialogueBox dialogueBoxComponent;
    public List<DialogueText> sentences;

    private bool inDialogue = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        dialogueBoxComponent = dialogueBoxObj.GetComponent<DialogueBox>();

    }

    //Manager is going to listen to a bunch of events that then cause it to gain new sentences and 
    void Start()
    {
        clearPanel();
    }

    public IEnumerator StartDialogue(List<DialogueText> newSentences)
    {
        if (dialogueBoxObj.activeInHierarchy)
        {
            yield break;
        }

        dialogueBoxObj.SetActive(true);

        sentences = newSentences;
        inDialogue = true;

        DisplayNextSentence();
        yield return new WaitUntil(() => !inDialogue);
    }

    public void clearPanel()
    {
        dialogueBoxObj.SetActive(false);
        sentences.Clear();
        inDialogue = false;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueText sentence = sentences[0];
        sentences.RemoveAt(0); //Uses a list instead of a Queue so we can see it in the Unity Editor

        dialogueBoxComponent.SetLine(sentence);
    }

    void EndDialogue()
    {
        clearPanel();
    }

    void Update()
    {
        if (dialogueBoxComponent.FinishedLine())
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space))
            {
                if (inDialogue)
                {
                    DisplayNextSentence();
                }
            }
        }
    }
}

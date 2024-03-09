using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public GameObject dialogueScrim; //Blocks player interaction with the game while in dialogue
    public GameObject dialogueBoxObj;
    private DialogueBox dialogueBoxComponent;
    private List<DialogueText> sentences = new();

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
        ClearPanel();
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

    public void ClearPanel()
    {
        dialogueBoxObj.SetActive(false);
        sentences.Clear();
        inDialogue = false;
    }

    public void MoveBoxToBottom()
    {
        dialogueBoxObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        dialogueBoxObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
    }

    public void MoveBoxToTop()
    {
        dialogueBoxObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        dialogueBoxObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        dialogueBoxObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -113);
    }

    public void BlockPlayerClick()
    {
        dialogueScrim.SetActive(true);
    }

    public void UnblockPlayerClick()
    {
        dialogueScrim.SetActive(false);
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
        ClearPanel();
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
        if (inDialogue)
        {
            BlockPlayerClick();
        } else
        {
            UnblockPlayerClick();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }



    private DialogueBox activeDialogueBox;
    [SerializeField]
    private DialogueBox picturelessDialogueBoxComponent;
    [SerializeField]
    private DialogueBox pictureDialogueBox;
    private List<DialogueText> sentences = new();

    private bool inDialogue = false;
    private bool isSkipping = false;

    List<DialogueText> dialogueHistory = new();

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
        activeDialogueBox = pictureDialogueBox;
    }

    //Manager is going to listen to a bunch of events that then cause it to gain new sentences and 
    void Start()
    {
        ClearPanel();
    }

    public bool IsInDialogue()
    {
        return inDialogue;
    }

    public IEnumerator StartDialogue(List<DialogueText> newSentences)
    {
        if (activeDialogueBox.gameObject.activeInHierarchy)
        {
            yield break;
        }

        activeDialogueBox.gameObject.SetActive(true);

        sentences = newSentences;
        inDialogue = true;
        
        DisplayNextSentence(true);
        yield return new WaitUntil(() => !inDialogue);
    }


    void AddLineToHistory(DialogueText text)
    {
        dialogueHistory.Add(text);
    }

    public List<DialogueText> GetHistory()
    {
        return dialogueHistory;
    }


void ClearPanel()
    {
        activeDialogueBox.gameObject.SetActive(false);
        sentences.Clear();
        inDialogue = false;
    }

    public void MoveBoxToBottom()
    {
        pictureDialogueBox.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        pictureDialogueBox.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
        pictureDialogueBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        if (picturelessDialogueBoxComponent != null)
        {
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }

    public void MoveBoxToTop()
    {
        pictureDialogueBox.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        pictureDialogueBox.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        pictureDialogueBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * pictureDialogueBox.GetComponent<RectTransform>().rect.height);

        if (picturelessDialogueBoxComponent != null)
        {
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            picturelessDialogueBoxComponent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * picturelessDialogueBoxComponent.GetComponent<RectTransform>().rect.height);
        }
    }

    void DisplayNextSentence(bool playAudioImmediate)
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueText sentence = sentences[0];
        sentences.RemoveAt(0); //Uses a list instead of a Queue so we can see it in the Unity Editor

        AddLineToHistory(sentence);

        if (sentence.DisplayingImage == null && picturelessDialogueBoxComponent != null)
        {
            SetDialogueBoxActive(picturelessDialogueBoxComponent);
            picturelessDialogueBoxComponent.SetLine(sentence);

        } else
        {
            SetDialogueBoxActive(pictureDialogueBox);
            pictureDialogueBox.SetLine(sentence);
        }

        if (playAudioImmediate) AudioManager.Instance.PlaySFX("Page Flip");
        sentence.playSound();
    }

    void SetDialogueBoxActive(DialogueBox dialogueBox)
    {
        if (picturelessDialogueBoxComponent != null)
        {
            picturelessDialogueBoxComponent.gameObject.SetActive(dialogueBox == picturelessDialogueBoxComponent);
            pictureDialogueBox.gameObject.SetActive(dialogueBox == pictureDialogueBox);
            activeDialogueBox = dialogueBox;
        } else
        {
            pictureDialogueBox.gameObject.SetActive(true);
            activeDialogueBox = pictureDialogueBox;
        }
    }

    void EndDialogue()
    {
        ClearPanel();
    }

    void Update()
    {
        if (PauseMenu.IsPaused) return;

        bool longSkip = Input.GetKey(KeyCode.RightArrow);

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space) || longSkip)
        {
            if (inDialogue)
            {
                if (activeDialogueBox.FinishedLine())
                {
                    // Or operator prevents space/mouse0 unsetting it, only sets to true if there is dialogue to display
                    isSkipping |= longSkip && (sentences.Count > 0);
                    DisplayNextSentence(!longSkip);
                }
                else
                {
                    activeDialogueBox.StopScrollingText();
                }
            }
        }
        if (isSkipping && (Input.GetKeyUp(KeyCode.RightArrow) || !inDialogue))
        {
            isSkipping = false;
            AudioManager.Instance.PlaySFX("Page Flip");
        }
    }
}

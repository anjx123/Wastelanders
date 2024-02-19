using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This class handles Rolling Text behavior, Use this in place of TextMeshProUGUI components to set text
public class DialogueBox : MonoBehaviour
{
    public const string RENDERING_LAYER = "DialogueLayer";
    private const float DEFAULTROLLSPEED = 50f;

    [SerializeField] private TextMeshPro bodyText;
    [SerializeField] private TextMeshPro nameText;
    private string currentLine;

    public float rollingSpeed;

    private bool lineIsFinished;

    public void SetLine(DialogueText line)
    {
        bodyText.text = string.Empty;
        this.currentLine = line.BodyText;
        nameText.text = line.SpeakerName;
        StartDialogue();
    }


    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer dialogueBoxRenderer = GetComponent<SpriteRenderer>();
        dialogueBoxRenderer.sortingLayerName = RENDERING_LAYER;
        dialogueBoxRenderer.sortingOrder = 0;

        MeshRenderer bodyTextRenderer = bodyText.GetComponent<MeshRenderer>();
        bodyTextRenderer.sortingLayerName = RENDERING_LAYER;
        bodyTextRenderer.sortingOrder = 1;

        MeshRenderer nameTextRenderer = nameText.GetComponent<MeshRenderer>();
        nameTextRenderer.sortingLayerName = RENDERING_LAYER;
        nameTextRenderer.sortingOrder = 1;

        if (rollingSpeed == 0)
        {
            rollingSpeed = DEFAULTROLLSPEED;
        }
        bodyText.text = string.Empty;

    }

    // Update is called once per frame
    void Update()
    {
        if (bodyText.text == currentLine)
        {
            lineIsFinished = true;
        }
        else
        {
            lineIsFinished = false;
        }

        if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (bodyText.text != currentLine)
            {
                StopAllCoroutines();
                bodyText.text = currentLine;
            }
        }
    }


    void StartDialogue()
    {
        StartCoroutine(TypeLine());
    }

    //Effects: Adds the rolling dialogue effect to the text
    IEnumerator TypeLine()
    {

        int currentIndex = 0;
        string displayedText = "";

        while (currentIndex < currentLine.Length)
        {
            displayedText += currentLine[currentIndex];
            currentIndex++;

            bodyText.text = displayedText;

            yield return new WaitForSecondsRealtime(1f / rollingSpeed);
        }

    }

    public bool FinishedLine()
    {
        return lineIsFinished;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//This class handles Rolling Text behavior, Use this in place of TextMeshProUGUI components to set text
public class DialogueBox : MonoBehaviour
{
    public const string RENDERING_LAYER = "DialogueLayer";
    private const float DEFAULTROLLSPEED = 50f;

    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image displayingImage;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private string currentLine = string.Empty;

    public float rollingSpeed;

    private bool lineIsFinished;

    public void SetLine(DialogueText line)
    {
        if (line.DisplayingImage == null)
        {
            displayingImage.enabled = false;
        } else
        {
            aspectRatioFitter.aspectRatio = line.DisplayingImage.bounds.size.x / line.DisplayingImage.bounds.size.y;
            displayingImage.enabled = true;
            displayingImage.sprite = line.DisplayingImage;
        }
        bodyText.text = string.Empty;
        this.currentLine = line.BodyText;
        nameText.text = line.SpeakerName;
        StartDialogue();
    }

    private void Awake()
    {
        if (rollingSpeed == 0)
        {
            rollingSpeed = DEFAULTROLLSPEED;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StopScrollingText()
    {
        if (bodyText.text != currentLine)
        {
            StopAllCoroutines();
            bodyText.text = currentLine;
            lineIsFinished = true;
        }
    }


    void StartDialogue()
    {
        StartCoroutine(TypeLine());
    }

    //Effects: Adds the rolling dialogue effect to the text
    IEnumerator TypeLine()
    {

        lineIsFinished = false;
        int currentIndex = 0;
        string displayedText = "";

        while (currentIndex < currentLine.Length)
        {
            displayedText += currentLine[currentIndex];
            currentIndex++;

            bodyText.text = displayedText;

            yield return new WaitForSecondsRealtime(1f / rollingSpeed);
        }

        lineIsFinished = true;

    }

    public bool FinishedLine()
    {
        return lineIsFinished;
    }

}

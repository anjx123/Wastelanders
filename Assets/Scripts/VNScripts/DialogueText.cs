using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueText
{
    [SerializeField] private string bodyText;
    [SerializeField] private string speakerName;
    [SerializeField] private Sprite displayingImage;

    public string BodyText {  get { return bodyText; } set {  bodyText = value; } }
    public string SpeakerName { get {  return speakerName; } set {  speakerName = value; } }
    public Sprite DisplayingImage { get { return displayingImage; } set { displayingImage = value; } }

    DialogueText(string bodyText, string speakerName, Sprite givenImage)
    {
        this.bodyText = bodyText;
        this.speakerName = speakerName;
        this.displayingImage = givenImage;
    }

}

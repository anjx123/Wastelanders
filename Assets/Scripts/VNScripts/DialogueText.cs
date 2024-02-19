using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueText
{
    [SerializeField] private string bodyText;
    [SerializeField] private string speakerName;

    public string BodyText {  get { return bodyText; } set {  bodyText = value; } }
    public string SpeakerName { get {  return speakerName; } set {  speakerName = value; } }

    DialogueText(string bodyText, string speakerName)
    {
        this.bodyText = bodyText;
        this.speakerName = speakerName;
    }

}

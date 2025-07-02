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
    [SerializeField] private string sfx;

    private bool italics;
    private bool bold;
    public bool broadcastAnEvent = false;


    public bool Italics { get { return italics; } set { italics = value; } }
    public bool Bold { get { return bold; } set { bold = value; } }


    public string BodyText {  get { return bodyText; } set {  bodyText = value; } }
    public string SpeakerName { get {  return speakerName; } set {  speakerName = value; } }
    public Sprite DisplayingImage { get { return displayingImage; } set { displayingImage = value; } }

    public DialogueText(string bodyText, string speakerName, Sprite givenImage)
    {
        this.bodyText = bodyText;
        this.speakerName = speakerName;
        this.displayingImage = givenImage;
    }

    public void playSound() {
        if (sfx != null && sfx != "") {
            AudioManager.Instance.PlaySFX(sfx);
        }
    }
}

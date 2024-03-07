using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueWrapper", order = 1)]
public class DialogueWrapper : ScriptableObject
{
    public List<DialogueText> dialogue;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * It seems that for the most part, the level select scene components are mostly self managed
 * e.g. Button unlock by progression, etc.
 * For other events, such as level completion triggers, they can be handled here
 */
public class LevelSelectManager : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(InitializeDialogue());
    }

    IEnumerator InitializeDialogue()
    {
        Debug.Log("bruh1");
        yield return new WaitForSeconds(2f);
        Debug.Log("bruh2");
        yield return StartCoroutine(DialogueManager
            .Instance
            .StartDialogue(new List<DialogueText>
        {
            new DialogueText("Welcome to the level select screen!", "", null),
            new DialogueText("You can select a level to play by clicking on the buttons below.", "", null),
            new DialogueText("Some levels may be locked until you complete previous levels.", "", null),
            new DialogueText("Good luck and have fun!", "", null),
        }));
        yield return null;
    }

    void Update()
    {
        
    }
}

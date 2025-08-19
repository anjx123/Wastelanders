using LevelSelectInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BattleIntroEnum;

public class PreBounty_1 : MonoBehaviour
{
    [SerializeField] private GameObject jackie;
    [SerializeField] private GameObject ives;
    
    [SerializeField] private FadeScreenHandler fadeScreenHandler;
    
    [SerializeField] private List<DialogueText> bountyDiscussionDialogue;


    public void Start()
    {
        StartCoroutine(StartScene());
    }

    public IEnumerator StartScene()
    {
        fadeScreenHandler.SetDarkScreen();
        yield return new WaitForSeconds(1f);
        
        yield return fadeScreenHandler.FadeInLightScreen(1f);
    }
}

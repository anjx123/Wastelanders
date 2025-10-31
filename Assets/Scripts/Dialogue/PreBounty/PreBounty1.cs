using LevelSelectInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using static BattleIntroEnum;

public class PreBounty1 : MonoBehaviour
{
    [SerializeField] private GameObject jackie;
    [SerializeField] private GameObject ives;

    [SerializeField] private Transform ivesTarget;
    
    [SerializeField] private DialogueWrapper JackieReminiscingDialogue;
    [SerializeField] private DialogueWrapper BountyBoardDialogue;

    [SerializeField] private float ivesMoveSpeed = 6f;
    
    public void Start()
    {
        StartCoroutine(StartScene());
    }

    public IEnumerator StartScene()
    {
        UIFadeScreenManager.Instance.SetDarkScreen();
        yield return new WaitForSeconds(1f);
        
        yield return UIFadeScreenManager.Instance.FadeInLightScreen(1f);
        
        yield return DialogueManager.Instance.StartDialogue(JackieReminiscingDialogue.Dialogue);

        yield return DialogueSceneUtils.MoveCharacterToTarget(ives, ivesTarget, ivesMoveSpeed);
        
        ives.GetComponent<Animator>().speed = 0.3f;
        
        yield return new WaitForSeconds(1f);
        
        yield return DialogueManager.Instance.StartDialogue(BountyBoardDialogue.Dialogue);
        
        yield return UIFadeScreenManager.Instance.FadeInDarkScreen(1f);
    }
}

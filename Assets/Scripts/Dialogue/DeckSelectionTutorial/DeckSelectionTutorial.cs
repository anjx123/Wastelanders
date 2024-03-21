using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectionTutorial : MonoBehaviour
{
    [SerializeField] private FadeScreenHandler fadeHandler;

    [SerializeField] private DialogueWrapper selectYourCharacter;
    [SerializeField] private DialogueWrapper selectYourWeapon;
    [SerializeField] private DialogueWrapper editYourWeapon;
    [SerializeField] private DialogueWrapper selectYourActions;
    [SerializeField] private DialogueWrapper backButtonTutorial;

    [SerializeField] private List<CharacterSelect> lockedCharacters;
    [SerializeField] private List<WeaponSelect> lockedWeapons;


    private void Start()
    {
        StartCoroutine(ExecuteGameStart());
    }

    private IEnumerator ExecuteGameStart()
    {
        GameStateManager.shouldPlayDeckSelectionTutorial = true;
        if (GameStateManager.shouldPlayDeckSelectionTutorial == false) yield break;

        fadeHandler.SetDarkScreen();
        foreach (CharacterSelect character in lockedCharacters) {
            character.SetLockedState(true);
        }
        yield return StartCoroutine(fadeHandler.FadeInLightScreen(2f));
        yield return StartCoroutine(StartDialogueWithNextEvent(selectYourCharacter.Dialogue, () => { CharacterSelect.CharacterSelectedEvent += HandleCharacterSelected; } ));
    }

    private void HandleCharacterSelected(PlayerDatabase.PlayerName playerName)
    {
        CharacterSelect.CharacterSelectedEvent -= HandleCharacterSelected;
        StartCoroutine(StartDialogueWithNextEvent(selectYourWeapon.Dialogue, () => { WeaponSelect.WeaponSelectEvent += HandleWeaponSelected; }));
    }

    private void HandleWeaponSelected(WeaponSelect weaponSelect, CardDatabase.WeaponType type)
    {
        WeaponSelect.WeaponSelectEvent -= HandleWeaponSelected;
        StartCoroutine(StartDialogueWithNextEvent(editYourWeapon.Dialogue, () => { WeaponEdit.WeaponEditEvent += HandleWeaponEdited; }));
    }

    private void HandleWeaponEdited(CardDatabase.WeaponType type)
    {
        WeaponEdit.WeaponEditEvent -= HandleWeaponEdited;
        GameStateManager.shouldPlayDeckSelectionTutorial = false;
        //DeckSelectionManager.Instance.SetNextScene("Scene2");
        StartCoroutine(StartDialogueWithNextEvent(selectYourActions.Dialogue, () => { DeckSelectionManager.Instance.PlayerActionDeckModifiedEvent += HandleRunOutOfPoints; }));
    }
    private void HandleRunOutOfPoints(int points)
    {
        Debug.Log("Poitns i am getting" + points);
        if (points < 2)
        {
            DeckSelectionManager.Instance.PlayerActionDeckModifiedEvent -= HandleRunOutOfPoints;
            StartCoroutine(DialogueManager.Instance.StartDialogue(backButtonTutorial.Dialogue));
        }
    }


    //Helper to wait until dialogue is done, then start @param dialogue, then run a callback like setting up a new event. 
    private IEnumerator StartDialogueWithNextEvent(List<DialogueText> dialogue, Action callbackToRun)
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
        callbackToRun();
    }

}

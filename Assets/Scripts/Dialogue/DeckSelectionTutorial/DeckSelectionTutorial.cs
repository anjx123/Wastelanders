using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckSelectionTutorial : MonoBehaviour
{
    [SerializeField] private FadeScreenHandler fadeHandler;
    [SerializeField] private PlayerDatabase playerDatabase;
    [SerializeField] private CharacterSelect jackieSelect;

    [SerializeField] private DialogueWrapper selectYourCharacter;
    [SerializeField] private DialogueWrapper selectYourWeapon;
    [SerializeField] private DialogueWrapper editYourWeapon;
    [SerializeField] private DialogueWrapper selectYourActions;
    [SerializeField] private DialogueWrapper backButtonTutorial;

    [SerializeField] private List<CharacterSelect> lockedCharacters;
    [SerializeField] private List<WeaponSelect> lockedWeapons;

    [SerializeField] private List<WeaponEdit> weaponEditBoxCollidersToDisable;

    [SerializeField] private bool activateTutorial;

    private void Start()
    {
        StartCoroutine(ExecuteGameStart());
    }

    private void OnDestroy()
    {
        WeaponSelect.WeaponSelectEvent -= HandleWeaponSelected;
        CharacterSelect.CharacterSelectedEvent -= HandleCharacterSelected;
        WeaponEdit.WeaponEditEvent -= HandleWeaponEdited;
        DeckSelectionManager.Instance.PlayerActionDeckModifiedEvent -= HandleRunOutOfPoints;
    }

    private IEnumerator ExecuteGameStart()
    {
        if (GameStateManager.Instance.JustFinishedBeetleFight)
        {
            DeckSelectionManager.Instance.SetNextScene(GameStateManager.PRE_QUEEN_FIGHT);
            yield break;
        }


        if (GameStateManager.Instance.ShouldPlayDeckSelectionTutorial == false && !activateTutorial) yield break;

        NormalizeTutorialDecks();

        foreach (WeaponEdit boxCollider in weaponEditBoxCollidersToDisable)
        {
            boxCollider.GetComponent<BoxCollider2D>().enabled = false;
        }
        jackieSelect.GetComponent<BoxCollider2D>().enabled = false;
        fadeHandler.SetDarkScreen();
        foreach (CharacterSelect character in lockedCharacters) {
            character.SetLockedState(true);
        }
        foreach (WeaponSelect weapon in lockedWeapons)
        {
            weapon.SetLockedState(true);
        }
        yield return StartCoroutine(fadeHandler.FadeInLightScreen(2f));
        yield return StartCoroutine(StartDialogueWithNextEvent(selectYourCharacter.Dialogue, () => { jackieSelect.GetComponent<BoxCollider2D>().enabled = true; CharacterSelect.CharacterSelectedEvent += HandleCharacterSelected; } ));
    }

    private void HandleCharacterSelected(PlayerDatabase.PlayerName playerName)
    {
        CharacterSelect.CharacterSelectedEvent -= HandleCharacterSelected;
        StartCoroutine(StartDialogueWithNextEvent(selectYourWeapon.Dialogue, () => { WeaponSelect.WeaponSelectEvent += HandleWeaponSelected; }));
    }

    private void HandleWeaponSelected(WeaponSelect weaponSelect, CardDatabase.WeaponType type)
    {
        WeaponSelect.WeaponSelectEvent -= HandleWeaponSelected;
        StartCoroutine(StartDialogueWithNextEvent(editYourWeapon.Dialogue, () => {
            foreach (WeaponEdit boxCollider in weaponEditBoxCollidersToDisable)
            {
                boxCollider.GetComponent<BoxCollider2D>().enabled = true;
            }
            WeaponEdit.WeaponEditEvent += HandleWeaponEdited; }));
    }

    private void HandleWeaponEdited(CardDatabase.WeaponType type)
    {
        WeaponEdit.WeaponEditEvent -= HandleWeaponEdited;
        GameStateManager.Instance.ShouldPlayDeckSelectionTutorial = false;
        DeckSelectionManager.Instance.SetNextScene(GameStateManager.FROG_SLIME_FIGHT);
        StartCoroutine(StartDialogueWithNextEvent(selectYourActions.Dialogue, () => { DeckSelectionManager.Instance.PlayerActionDeckModifiedEvent += HandleRunOutOfPoints; }));
    }
    private void HandleRunOutOfPoints(int points)
    {
        if (points < 2)
        {
            DeckSelectionManager.Instance.PlayerActionDeckModifiedEvent -= HandleRunOutOfPoints;
            StartCoroutine(DialogueManager.Instance.StartDialogue(backButtonTutorial.Dialogue));
        }
    }




    //Completely removes the PISTOL deck from jackie
    private void NormalizeTutorialDecks()
    {
        playerDatabase.JackieData.selectedWeapons.Remove(CardDatabase.WeaponType.PISTOL);
        SerializableWeaponListEntry pistolDeck = playerDatabase.JackieData.playerDeck.FirstOrDefault(deck => deck.key == CardDatabase.WeaponType.PISTOL);
        pistolDeck.value = new List<SerializableTuple<string, bool>>();
        SerializableTuple<CardDatabase.WeaponType, SerializableTuple<int, int>> pointsAvailableForPistol = playerDatabase.JackieData.playerWeaponProficiency.FirstOrDefault(proficiency => proficiency.Item1 == CardDatabase.WeaponType.PISTOL);
        pointsAvailableForPistol.Item2.Item1 = 0;
    }


    //Helper to wait until dialogue is done, then start @param dialogue, then run a callback like setting up a new event. 
    private IEnumerator StartDialogueWithNextEvent(List<DialogueText> dialogue, Action callbackToRun)
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsInDialogue());
        yield return StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
        callbackToRun();
    }

}

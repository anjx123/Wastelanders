using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

//Static Class that keeps track of static values representing general Game states
public static class GameStateManager
{
    public static bool shouldPlayDeckSelectionTutorial = false;

    public static bool jumpIntoFrogAndSlimeFight = false;

    public static bool jumpIntoBeetleFight = false;

    public static bool jumpIntoQueenFight = false;

    public enum Scenes
    {
        Tutorial,
        FrogSlime,
        Beetle,
        QueenBeetle,
        Unknown
    }

    public static Scenes GetSceneEnum(string s)
    {
        switch(s)
        {
            case "TutorialScene":
                return Scenes.Tutorial;
            case "Scene2":
                return Scenes.FrogSlime;
            case "BeetleCombatScene":
                return Scenes.Beetle;
            default: 
                return Scenes.Unknown;
        }
    }

    public static void SkipDialogue(string s)
    {
        Scenes x = GetSceneEnum(s);
        switch(x)
        {
            case Scenes.Tutorial:
                break;
            case Scenes.FrogSlime:
                GameStateManager.jumpIntoFrogAndSlimeFight = true;
                break;
            case Scenes.Beetle:
                GameStateManager.jumpIntoBeetleFight = true;
                break;
            default: break;
        }
    }
}

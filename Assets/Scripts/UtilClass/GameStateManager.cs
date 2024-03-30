
using UnityEngine;
using UnityEngine.SceneManagement;

//Static Class that keeps track of static values representing general Game states
public static class GameStateManager
{
    public static bool shouldPlayDeckSelectionTutorial = false;

    public static bool jumpIntoFrogAndSlimeFight = false;

    public static bool jumpIntoBeetleFight = false;

    public static bool jumpIntoQueenFight = false;

    //If we just finished beetle fight, we go directly into queen fight after the back button is hit
    public static bool justFinishedBeetleFight = false;

    public static string nameOfRestartedLevel = "";


    public static readonly string MAIN_MENU_NAME = "MainMenu";
    public static readonly string SELECTION_SCREEN_NAME = "SelectionScreen";

    public static readonly string LEVEL_SELECT_NAME = "LevelSelect";

    public static readonly string PRE_QUEEN_FIGHT = "PreQueenFightScene";
    public static readonly string POST_QUEEN_FIGHT = "PostQueenBeetle";
    public static readonly string CREDITS = "Credits";
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

    public static void Restart(string sceneName)
    {
        GameStateManager.nameOfRestartedLevel = sceneName;
        SceneManager.LoadScene("RestartScene");
    }

    public static void SavePlayerDatabase(PlayerDatabase playerDatabase)
    {
        string json = JsonUtility.ToJson(playerDatabase);
        PlayerPrefs.SetString("PlayerDatabase", json);
    }

    public static PlayerDatabase LoadPlayerDatabase()
    {
        string json = PlayerPrefs.GetString("PlayerDatabase");
        return JsonUtility.FromJson<PlayerDatabase>(json);
    }

}

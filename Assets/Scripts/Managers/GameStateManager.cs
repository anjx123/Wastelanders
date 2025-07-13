
using BountySystem;
using System.Runtime.CompilerServices;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

//Singleton Class that keeps track of values representing general Game states
public class GameStateManager : PersistentSingleton<GameStateManager>, IBind<GameStateData>
{
    public static readonly bool IS_DEVELOPMENT = true;

    //Fields for persistence
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    private GameStateData data;

    public GameStateData Data 
    { 
        get
        {
            // Data should only be nullable during development where you can open a scene from any place
            if (data == null)
            {
                SaveLoadSystem.Instance.LoadGameStateInformation();
            }

            return data;
        }
        set
        {
            data = value;
        }
    }

    /*
     * If we just finished beetle fight, we go directly into queen fight after the back button is hit
     * This is a temporary flag that is set to check this state to directly transition
     */
    public bool JustFinishedBeetleFight = false;
    
    /*
     * Another temporary state that determines whether the tutorial should be played
     */
    public bool ShouldPlayDeckSelectionTutorial = false;

    /*
     * Temporary flag to be set and read by end of combat scene, when the player restarts and should skip dialogue
     * Is set by GameOver prefab upon restart, and read by dialogue classes
     * Dialogue classes should reset this value when read, such that it does not cause unexpected behaviour in upcoming scenes
     */
    public bool JumpToCombat = false;

    public float CurrentLevelProgress
    {
        get { return (IS_DEVELOPMENT) ? 100f : Data.CurrentLevelProgress; }
        set => Data.CurrentLevelProgress = value;
    }

    public void Restart()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void Bind(GameStateData bindedData)
    {
        this.Data = bindedData;
        this.Data.Id = bindedData.Id;
    }

    public void LoadScene(string scene) 
    {
        SceneManager.LoadScene(scene);
        SaveLoadSystem.Instance.SaveGame();
    }

    public const string MAIN_MENU_NAME = "MainMenu";
    public const string SELECTION_SCREEN_NAME = "SelectionScreen";

    public const string LEVEL_SELECT_NAME = "LevelSelect";
    public const string CONTRACT_SELECT_NAME = "ContractSelect";

    public const string TUTORIAL_FIGHT = "TutorialScene";
    public const string FROG_SLIME_FIGHT = "FrogSlimeFight";
    public const string BEETLE_FIGHT = "BeetleFightScene";
    public const string PRE_QUEEN_FIGHT = "PreQueenFightScene";
    public const string POST_QUEEN_FIGHT = "PostQueenBeetle";
    public const string CREDITS = "Credits";

    public const string PRINCESS_FROG_BOUNTY = "PrincessFrogCombatScene";
}


[System.Serializable]
public class GameStateData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    /*
     * This is the last level progress the player was on
     * It's current application is to give a relative start to it's current state
     *  such that we can deduce what is newly seen by the player and what isn't
     * Currently, it's synched to CurrentLevelProgress upon loading the game
     */
    [field: SerializeField] public float LastLevelProgress { get; set; } = 0f;

    /*
     * This is the current state that the player is at
     * The associated values for this should be from [LevelSelectInformation.levelId]
     */
    [field: SerializeField] public float CurrentLevelProgress { get; set; } = 0f;


    public override string ToString()
    {
        return "Id: " + Id +
            " Hexcode: " + RuntimeHelpers.GetHashCode(this) +
            " Last player level progress: " + LastLevelProgress +
            " Current player level progress: " + CurrentLevelProgress;
    }
}

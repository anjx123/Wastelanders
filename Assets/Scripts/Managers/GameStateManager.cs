
using BountySystem;
using System.Runtime.CompilerServices;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

//Singleton Class that keeps track of values representing general Game states
public class GameStateManager : PersistentSingleton<GameStateManager>, IBind<GameStateData>
{
    public static readonly bool IS_DEVELOPMENT = false;

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

    public bool ShouldPlayDeckSelectionTutorial 
    {
        get { return Data.ShouldPlayDeckSelectionTutorial; } 
        set => Data.ShouldPlayDeckSelectionTutorial = value; 
    }

    public bool JumpIntoFrogAndSlimeFight 
    {
        get { return Data.JumpIntoFrogAndSlimeFight; }
        set => Data.JumpIntoFrogAndSlimeFight = value;
    }

    public bool JumpIntoBeetleFight
    {
        get { return Data.JumpIntoBeetleFight; }
        set => Data.JumpIntoBeetleFight = value;
    }

    public bool JumpIntoQueenFight
    {
        get { return Data.JumpIntoQueenFight; }
        set => Data.JumpIntoQueenFight = value;
    }

    //If we just finished beetle fight, we go directly into queen fight after the back button is hit
    public bool JustFinishedBeetleFight
    {
        get { return Data.JustFinishedBeetleFight; }
        set => Data.JustFinishedBeetleFight = value;
    }

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

    [field: SerializeField] public bool ShouldPlayDeckSelectionTutorial { get; set; } = false;

    [field: SerializeField] public bool JumpIntoFrogAndSlimeFight { get; set; } = false;

    [field: SerializeField] public bool JumpIntoBeetleFight { get; set; } = false;

    [field: SerializeField] public bool JumpIntoQueenFight { get; set; } = false;

    [field: SerializeField] public bool JustFinishedBeetleFight { get; set; } = false;

    [field: SerializeField] public float CurrentLevelProgress { get; set; } = 0f;  
    public override string ToString()
    {
        return "Id: " + Id +
            " Hexcode: " + RuntimeHelpers.GetHashCode(this) +
            " ShouldPlayDeckSelectionTutorial: " + ShouldPlayDeckSelectionTutorial +
            " JumpIntoFrogAndSlimeFight: " + JumpIntoFrogAndSlimeFight +
            " JumpIntoBeetleFight: " + JumpIntoBeetleFight +
            " JumpIntoQueenFight: " + JumpIntoQueenFight +
            " JustFinishedBeetleFight: " + JustFinishedBeetleFight +
            " Current player level progress: " + CurrentLevelProgress;
    }
}

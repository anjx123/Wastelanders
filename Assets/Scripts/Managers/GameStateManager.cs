
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

//Static Class that keeps track of static values representing general Game states
public class GameStateManager : PersistentSingleton<GameStateManager>, IBind<GameStateData>
{
    //Fields for persistence
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [SerializeField] GameStateData data;

    public bool ShouldPlayDeckSelectionTutorial 
    {
        get { return data.ShouldPlayDeckSelectionTutorial; } 
        set => data.ShouldPlayDeckSelectionTutorial = value; 
    }

    public bool JumpIntoFrogAndSlimeFight 
    {
        get { return data.JumpIntoFrogAndSlimeFight; }
        set => data.JumpIntoFrogAndSlimeFight = value;
    }

    public bool JumpIntoBeetleFight
    {
        get { return data.JumpIntoBeetleFight; }
        set => data.JumpIntoBeetleFight = value;
    }

    public bool JumpIntoQueenFight
    {
        get { return data.JumpIntoQueenFight; }
        set => data.JumpIntoQueenFight = value;
    }

    //If we just finished beetle fight, we go directly into queen fight after the back button is hit
    public bool JustFinishedBeetleFight
    {
        get { return data.JustFinishedBeetleFight; }
        set => data.JustFinishedBeetleFight = value;
    }
    public void Restart()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void Bind(GameStateData data)
    {
        this.data = data;
        this.data.Id = data.Id;
    }

    public const string MAIN_MENU_NAME = "MainMenu";
    public const string SELECTION_SCREEN_NAME = "SelectionScreen";

    public const string LEVEL_SELECT_NAME = "LevelSelect";

    public const string TUTORIAL_FIGHT = "TutorialScene";
    public const string FROG_SLIME_FIGHT = "Scene2";
    public const string BEETLE_FIGHT = "BeetleFightScene";
    public const string PRE_QUEEN_FIGHT = "PreQueenFightScene";
    public const string POST_QUEEN_FIGHT = "PostQueenBeetle";
    public const string CREDITS = "Credits";
}


[System.Serializable]
public class GameStateData : ISaveable
{
    public SerializableGuid Id { get; set; }

    public bool ShouldPlayDeckSelectionTutorial { get; set; } = false;

    public bool JumpIntoFrogAndSlimeFight { get; set; } = false;

    public bool JumpIntoBeetleFight { get; set; } = false;

    public bool JumpIntoQueenFight { get; set; } = false;

    public bool JustFinishedBeetleFight { get; set; } = false;
}

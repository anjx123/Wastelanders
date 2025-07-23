using LevelSelectInformation;
using System.Collections.Generic;
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

    /*
     * Temporary flag to be set and read by end of combat scene, when the player restarts and should skip dialogue
     * Is set by GameOver prefab upon restart, and read by dialogue classes
     * Dialogue classes should reset this value when read, such that it does not cause unexpected behaviour in upcoming scenes
     */
    public bool JumpToCombat = false;

    public void UpdateLevelProgress(ILevelSelectInformation level)
    {
        CurrentLevelProgress = Mathf.Max(CurrentLevelProgress, level.LevelID);
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
}


[System.Serializable]
public class GameStateData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    /*
     * This is the current state that the player is at
     * The associated values for this should be from [LevelSelectInformation.levelId]
     */
    [field: SerializeField] public float CurrentLevelProgress { get; set; } = 0f;


    public override string ToString()
    {
        var items = new List<string>
        {
            "Id: " + Id,
            "Hexcode: " + RuntimeHelpers.GetHashCode(this),
            "Current player level progress: " + CurrentLevelProgress
        };
        return string.Join(",", items);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneData : Enum<SceneData>
{
    public abstract string SceneName { get; }
    public abstract SceneAudio GetAudio(AudioDatabase database);
    public virtual Type[] RequiredManagerTypes => Array.Empty<Type>();

    public class MainMenu : SceneData
    {
        public override string SceneName => "MainMenu";
        public override SceneAudio GetAudio(AudioDatabase database) => database.MainMenu;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class SelectionScreen : SceneData
    {
        public override string SceneName => "SelectionScreen";
        public override SceneAudio GetAudio(AudioDatabase database) => database.MainMenu;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class LevelSelect : SceneData
    {
        public override string SceneName => "LevelSelect";
        public override SceneAudio GetAudio(AudioDatabase database) => database.MainMenu;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class ContractSelect : SceneData
    {
        public override string SceneName => "ContractSelect";
        public override SceneAudio GetAudio(AudioDatabase database) => database.MainMenu;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class Credits : SceneData
    {
        public override string SceneName => "Credits";
        public override SceneAudio GetAudio(AudioDatabase database) => database.Credits;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class TutorialFight : SceneData
    {
        public override string SceneName => "TutorialScene";
        public override SceneAudio GetAudio(AudioDatabase database) => database.TutorialFight;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class FrogSlimeFight : SceneData
    {
        public override string SceneName => "FrogSlimeFight";
        public override SceneAudio GetAudio(AudioDatabase database) => database.FrogSlimeFight; 
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class BeetleFight : SceneData
    {
        public override string SceneName => "BeetleFightScene";
        public override SceneAudio GetAudio(AudioDatabase database) => database.BeetleFight;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class PreQueenFight : SceneData
    {
        public override string SceneName => "PreQueenFightScene";
        public override SceneAudio GetAudio(AudioDatabase database) => database.PreQueenFight;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class PostQueenFight : SceneData
    {
        public override string SceneName => "PostQueenBeetle";
        public override SceneAudio GetAudio(AudioDatabase database) => database.PostQueenFight;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    public class PrincessFrogBounty : SceneData
    {
        public override string SceneName => "PrincessFrogCombatScene";
        public override SceneAudio GetAudio(AudioDatabase database) => database.PrincessFrogBounty;
        public override Type[] RequiredManagerTypes => new[] { typeof(AudioManager) };
    }

    private static readonly Dictionary<string, SceneData> _sceneLookup = new();
    
    static SceneData()
    {
        foreach (var sceneDataItem in Values)
        {
            if (!_sceneLookup.ContainsKey(sceneDataItem.SceneName))
            {
                _sceneLookup.Add(sceneDataItem.SceneName, sceneDataItem);
            }
            else
            {
                Debug.LogError($"Duplicate SceneName '{sceneDataItem.SceneName}' found in SceneData subclasses. " +
                               $"Each SceneData must have a unique SceneName.");
            }
        }
    }
    public static SceneData FromSceneName(string sceneName)
    {
        _sceneLookup.TryGetValue(sceneName, out var sceneData);
        if (sceneData == null) Debug.LogError("The scene hasn't been found in scene data, please add an entry.");
        return sceneData;
    }

    public static SceneData Get<T>() where T : SceneData => ParseFromType(typeof(T));
}
using System.Collections.Generic;
using UnityEngine;

public abstract class EpilogueSceneData : Enum<EpilogueSceneData>
{
    public abstract string EpilogueTitle { get; }
    public abstract int BountyRequirement { get; }
    public abstract SceneData SceneData { get; }

    public new static IEnumerable<EpilogueSceneData> Values => Enum<EpilogueSceneData>.Values;

    public delegate void EpilogueSceneDataDelegate(string sceneName);
    public static event EpilogueSceneDataDelegate EpilogueSceneDataEvent;

    public void UponSelectedEvent()
    {
        EpilogueSceneDataEvent?.Invoke(SceneData.SceneName);
    }

    public class SampleEpilogueSceneA : EpilogueSceneData
    {
        public override string EpilogueTitle => "Scene A";
        public override int BountyRequirement => 0;
        public override SceneData SceneData => SceneData.Get<SceneData.MainMenu>();
    }

    public class SampleEpilogueSceneB : EpilogueSceneData
    {
        public override string EpilogueTitle => "Scene B";
        public override int BountyRequirement => 1;
        public override SceneData SceneData => SceneData.Get<SceneData.LevelSelect>();
    }
    public class SampleEpilogueSceneC : EpilogueSceneData
    {
        public override string EpilogueTitle => "Scene C";
        public override int BountyRequirement => 3;
        public override SceneData SceneData => SceneData.Get<SceneData.Credits>();
    }

}
using System;
using UnityEngine;

namespace DialogueScripts
{
    [Serializable]
    public abstract class DialogueEvents
    {
        public abstract void Execute();
    };

    public class SetSpeaker: DialogueEvents, IEvent
    {
        [SerializeField] private CharacterActor actor = CharacterActor.Jackie;
        public CharacterActor Actor => actor;
        public override void Execute() => this.Invoke();
    }

    public enum CharacterActor
    {
        Jackie = 10,
        Cam = 20,
        Ives = 30 ,
        None = 1000,
    }

    public class SpriteChange : DialogueEvents, IEvent
    {
        [SerializeField] private CharacterActor actor = CharacterActor.Jackie;
        [SerializeField] private Sprite sprite;
        public CharacterActor Actor => actor;
        public Sprite Sprite => sprite;

        public override void Execute() => this.Invoke();
    }


    public class ActorAction : DialogueEvents, IEvent
    {
        [SerializeField] private CharacterActor actor = CharacterActor.Jackie;
        [SerializeField] private CharacterActions action = CharacterActions.SetLeft;
        [SerializeField] private float duration = 1.0f;
        public CharacterActor Actor => actor;
        public CharacterActions Action => action;
        public float Duration => duration;
        public override void Execute() => this.Invoke();
    }

    public enum CharacterActions
    {
        SetLeft = 10,
        SetMiddle = 20,
        SetRight = 30,
        SetOffscreenLeft = 40,
        SetOffscreenRight = 50,
        FadeIn = 60,
        FadeOut = 70,
    }

    /// Change the vertical positioning of the dialogue box. 
    public class VerticalLayoutChange : DialogueEvents, IEvent
    {
        [SerializeField] private Layout layout = Layout.Lower;
        public Layout Layout => layout;
        public override void Execute() => this.Invoke();
    }

    public enum Layout
    {
        Lower = 10,
        Upper = 20
    }

    /// Automatically advances this dialogue entry after it finishes. 
    public class AutoAdvanceAfter : DialogueEvents, IEvent
    {
        [SerializeField] private float time = 0.5f;
        public float Time => time;
        public override void Execute() => this.Invoke();
    }
}

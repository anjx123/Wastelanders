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
        [SerializeField] private CharacterActor actor;
        public CharacterActor Actor => actor;
        public override void Execute() => this.Invoke();
    }

    public enum CharacterActor
    {
        Jackie,
        Cam,
        Ives,
        None,
    }

    public class SpriteChange : DialogueEvents, IEvent
    {
        [SerializeField] private CharacterActor actor;
        [SerializeField] private Sprite sprite;
        public CharacterActor Actor => actor;
        public Sprite Sprite => sprite;

        public override void Execute() => this.Invoke();
    }


    public class ActorAction : DialogueEvents, IEvent
    {
        [SerializeField] private CharacterActor actor;
        [SerializeField] private CharacterActions action;
        [SerializeField] private float duration = 1.0f;
        public CharacterActor Actor => actor;
        public CharacterActions Action => action;
        public float Duration => duration;
        public override void Execute() => this.Invoke();
    }

    public enum CharacterActions
    {
        SetLeft,
        SetMiddle,
        SetRight,
        SetOffscreenLeft,
        SetOffscreenRight,
        FadeIn,
        FadeOut,
    }

    /// Change the vertical positioning of the dialogue box. 
    public class VerticalLayoutChange : DialogueEvents, IEvent
    {
        [SerializeField] private Layout layout = Layout.LOWER;
        public Layout Layout => layout;
        public override void Execute() => this.Invoke();
    }

    public enum Layout
    {
        LOWER,
        UPPER
    }

    /// Automatically advances this dialogue entry after it finishes. 
    public class AutoAdvanceAfter : DialogueEvents, IEvent
    {
        [SerializeField] private float time = 0.5f;
        public float Time => time;
        public override void Execute() => this.Invoke();
    }
}

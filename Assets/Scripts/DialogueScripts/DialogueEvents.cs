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
        public CharacterActor actor = CharacterActor.Jackie;
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
        public CharacterActor actor = CharacterActor.Jackie;
        public Sprite sprite;
        public override void Execute() => this.Invoke();
    }

    public class ActorAction : DialogueEvents, IEvent
    {
        public CharacterActor actor = CharacterActor.Jackie;
        public CharacterActions action = CharacterActions.SetLeft;
        public float duration = 1.0f;
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
        public Layout Layout = Layout.Lower;
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
        public float Time = 0.5f;
        public override void Execute() => this.Invoke();
    }
}

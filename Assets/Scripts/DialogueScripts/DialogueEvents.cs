using System;
using UnityEngine;

namespace DialogueScripts
{
    [Serializable]
    public abstract class DialogueEvents
    {
        public abstract void Execute();
    };

    public class SpriteChange : DialogueEvents
    {
        [SerializeField] private SpriteRenderer actor;
        [SerializeField] private Sprite sprite;

        public override void Execute()
        {
            actor.sprite = sprite;
        }
    }

    public class MoveCharacter : DialogueEvents
    {
        [SerializeField] private Transform actor;
        [SerializeField] private Transform transform;

        public override void Execute()
        {

        }
    }

    /// Change the vertical positioning of the dialogue box. 
    public class VerticalLayoutChange : DialogueEvents, IEvent
    {
        [SerializeField] private Layout layout;
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
        [SerializeField] private float time;
        public float Time => time;
        public override void Execute() => this.Invoke();
    }
}

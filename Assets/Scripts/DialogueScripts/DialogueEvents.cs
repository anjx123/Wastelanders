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

    public class SetAuto : DialogueEvents
    {
        public override void Execute()
        {

        }
    }

}

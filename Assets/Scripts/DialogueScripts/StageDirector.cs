using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueScripts
{
    public class StageDirector : MonoBehaviour
    {
        [Header("Stage Configuration")] [SerializeField]
        private Transform stageRoot;

        [SerializeField] private DialogueActor genericActorPrefab;

        [Header("Stage Anchors")] 
        [SerializeField] private Transform offScreenLeft;
        [SerializeField] private Transform leftPos;
        [SerializeField] private Transform centerPos;
        [SerializeField] private Transform rightPos;
        [SerializeField] private Transform offScreenRight;

        private readonly Dictionary<CharacterActor, DialogueActor> activeActors = new();

        private void Awake()
        {
            this.Subscribe<SpriteChange>(OnSpriteChange);
            this.Subscribe<ActorAction>(OnActorAction);
            this.Subscribe<SetSpeaker>(OnSetSpeaker);
        }


        private void OnSetSpeaker(SetSpeaker ss)
        {
            _ = GetOrSummonActor(ss.actor);
            foreach ((CharacterActor characterProfile, DialogueActor actorInstance) in activeActors)
            {
                actorInstance.SetSpeaker(isSpeaking: characterProfile == ss.actor);
            }
        }

        private void OnActorAction(ActorAction mc)
        {
            var actor = GetOrSummonActor(mc.actor);

            _ = mc.action switch
            {
                CharacterActions.SetLeft => actor.MoveTo(leftPos.position, mc.duration),
                CharacterActions.SetRight => actor.MoveTo(rightPos.position, mc.duration),
                CharacterActions.SetMiddle => actor.MoveTo(centerPos.position, mc.duration),
                CharacterActions.SetOffscreenLeft => actor.MoveTo(offScreenLeft.position, mc.duration),
                CharacterActions.SetOffscreenRight => actor.MoveTo(offScreenRight.position, mc.duration),
                CharacterActions.FadeIn => actor.FadeActor(true, mc.duration),
                CharacterActions.FadeOut => actor.FadeActor(false, mc.duration),
                _ => throw new ArgumentOutOfRangeException(nameof(mc.action), mc.action, "Unhandled CharacterAction")
            };
        }

        private void OnSpriteChange(SpriteChange sc)
        {
            GetOrSummonActor(sc.actor).ChangeSprite(sc.sprite);
        }

        private DialogueActor GetOrSummonActor(CharacterActor actor)
        {
            if (activeActors.TryGetValue(actor, out var ac)) return ac;

            DialogueActor newActor = Instantiate(genericActorPrefab, stageRoot);
            newActor.transform.position = offScreenLeft.position;

            activeActors.Add(actor, newActor);
            return newActor;
        }
    }
}
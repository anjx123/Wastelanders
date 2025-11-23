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
            foreach (var kvp in activeActors)
            {
                CharacterActor characterProfile = kvp.Key;
                DialogueActor actorInstance = kvp.Value;

                bool isSpeaking = characterProfile == ss.Actor;

                actorInstance.SetSpeaker(isSpeaking);

                if (isSpeaking)
                {
                    actorInstance.transform.SetAsLastSibling();
                }
            }
        }

        private void OnActorAction(ActorAction mc)
        {
            var actor = GetOrSummonActor(mc.Actor);

            _ = mc.Action switch
            {
                CharacterActions.SetLeft => actor.MoveTo(leftPos.position, mc.Duration),
                CharacterActions.SetRight => actor.MoveTo(rightPos.position, mc.Duration),
                CharacterActions.SetMiddle => actor.MoveTo(centerPos.position, mc.Duration),
                CharacterActions.SetOffscreenLeft => actor.MoveTo(offScreenLeft.position, mc.Duration),
                CharacterActions.SetOffscreenRight => actor.MoveTo(offScreenRight.position, mc.Duration),
                CharacterActions.FadeIn => actor.FadeActor(true, mc.Duration),
                CharacterActions.FadeOut => actor.FadeActor(false, mc.Duration),
                _ => throw new ArgumentOutOfRangeException(nameof(mc.Action), mc.Action, "Unhandled CharacterAction")
            };
        }

        private void OnSpriteChange(SpriteChange sc)
        {
            GetOrSummonActor(sc.Actor).ChangeSprite(sc.Sprite);
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleIntro : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator backgroundAnimator;
    private const string backgroundAnimation = "BackgroundIntro";

    public static BattleIntro Build(Camera camera)
    {
        BattleIntro battleIntro = SceneInitializer.Instance.InstantiatePrefab(SceneInitializer.Instance.InitializablePrefabs.battleIntro);
        battleIntro.canvas.worldCamera = camera;
        battleIntro.canvas.sortingLayerName = GameStateManager.SORTING_LAYER_TOP;
        return battleIntro;
    }

    public virtual void PlayAnimation(BattleIntroEnum animationEnum)
    {
        animator.SetTrigger(animationEnum.animationName);
        backgroundAnimator.SetTrigger(backgroundAnimation);
    }
}

public abstract class BattleIntroEnum : Enum<BattleIntroEnum>
{
    public abstract string animationName { get; }

    public class ClashIntro : BattleIntroEnum
    {
        public override string animationName => "ClashIntro";
    }

    public class TutorialIntro : BattleIntroEnum
    {
       public override string animationName => "TutorialIntro";
    }

    public static BattleIntroEnum Get<T>() where T : BattleIntroEnum => ParseFromType(typeof(T));
}
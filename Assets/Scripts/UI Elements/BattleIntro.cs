using Unity.VisualScripting;
using UnityEngine;


public class BattleIntro : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator backgroundAnimator;
    private const string backgroundAnimation = "BackgroundIntro";

    private void Awake()
    {
        canvas.sortingOrder = UISortOrder.CombatIntro.GetOrder();
        this.Subscribe<BattleIntroEvent>(PlayAnimation);
    }

    private void PlayAnimation(BattleIntroEvent animationEvent)
    {
        animator.SetTrigger(animationEvent.AnimationEnum.AnimationName);
        backgroundAnimator.SetTrigger(backgroundAnimation);
    }
}

public record BattleIntroEvent(BattleIntroEnum AnimationEnum) : IEvent;


public abstract class BattleIntroEnum : Enum<BattleIntroEnum>
{
    public abstract string AnimationName { get; }

    public class ClashIntro : BattleIntroEnum
    {
        public override string AnimationName => "ClashIntro";
    }

    public class TutorialIntro : BattleIntroEnum
    {
       public override string AnimationName => "TutorialIntro";
    }

    public static BattleIntroEnum Get<T>() where T : BattleIntroEnum => ParseFromType(typeof(T));
}
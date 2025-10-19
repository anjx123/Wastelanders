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
        this.AddComponent<BattleIntroEventHandler>()
            .Subscribe(PlayAnimation);
    }

    private void PlayAnimation(BattleIntroEvent animationEvent)
    {
        animator.SetTrigger(animationEvent.AnimationEnum.animationName);
        backgroundAnimator.SetTrigger(backgroundAnimation);
    }

    private class BattleIntroEventHandler : EventHandler<BattleIntroEvent> { }
}

public record BattleIntroEvent(BattleIntroEnum AnimationEnum) : IEvent;


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
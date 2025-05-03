
using UnityEngine;

public class Spit : FrogAttacks, IPlayableFrogCard
{
    [SerializeField] private AnimationClip animationClip;
    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        Speed = 4;
        CostToAddToDeck = 1;

        myName = "Spit";
        description = "Gross!";
    }
    public override void OnHit()
    {
        IPlayableEnemyCard.ApplyForeignAttackAnimation(Origin, animationClip, FROG_ATTACK_NAME);
        base.OnHit();
    }
}

using System.Collections;
using UnityEngine;

public class Pincer : BeetleAttacks
{
    public const string PINCER_SOUND_EFFECT_NAME = "Pincer Cut";

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 3;
        
        Speed = 4;

        description = "Ouch!";

        myName = "Pincer";
        CardType = CardType.MeleeAttack;
    }

    public override void OnHit()
    {
        AudioManager.Instance.PlaySFX(PINCER_SOUND_EFFECT_NAME);
        base.OnHit();
    }

    public override void CardIsUnstaggered()
    {
        if (Origin.HasAnimationParameter("IsAttacking"))
        {
            Origin.AttackAnimation("IsAttacking");
        }
        if (Origin.IsFacingRight())
        {
            StartCoroutine(MoveBasedOnDirection(/* isFacingRight = */true));
        } else
        {
            StartCoroutine(MoveBasedOnDirection(/* isFacingRight = */ false));
        }
    }

    protected IEnumerator MoveBasedOnDirection(bool isFacingRight)
    {
        Vector3 moveRight = new Vector3(0.3f, 0, 0);
        float moveTime = 0.6f;
        

        if (isFacingRight)
        {
            Vector3 originalPosition = Origin.myTransform.position;
            Origin.myTransform.position = originalPosition + moveRight;
            yield return new WaitForSeconds(moveTime);
            Origin.myTransform.position = originalPosition;
        }
        else
        {
            Vector3 originalPosition = Origin.myTransform.position;
            Origin.myTransform.position = originalPosition - moveRight;
            yield return new WaitForSeconds(moveTime);
            Origin.myTransform.position = originalPosition;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSmash : SlimeAttacks
{
#nullable enable
    [SerializeField]
    private List<Sprite> animationFrame = new();

    public override void ExecuteActionEffect()
    {

    }

   

    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;
        Speed = 2;

        myName = "Stack Smash";
        description = "If this attack is unstaggered, attack again";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }


    //@Author: Anrui. Called by ActionClass.OnHit() 
    public override void CardIsUnstaggered()
    {
        // branhces separated for mem leaks 
        if (proto && activeDupCardInstance == null)
        {
            activeDupCardInstance = Instantiate(duplicateCardInstance!.GetComponent<StackSmashDuplicate>()); 
            ((StackSmashDuplicate)activeDupCardInstance).proto = false;
            ((StackSmashDuplicate)activeDupCardInstance).duplicateCardInstance = null;
            activeDupCardInstance.transform.position = new Vector3(-10, 10, 10);
        }
        if (proto)
        {
            SlimeStack origin = (SlimeStack)Origin;
            activeDupCardInstance!.Origin = origin;
            activeDupCardInstance.Target = Target;
            BattleQueue.BattleQueueInstance.InsertDupEnemyAction(activeDupCardInstance);
        }
        base.CardIsUnstaggered();
    }
    public override void OnHit()
    {
        onHitWasCalled = true;
        StartCoroutine(AttackAnimation(base.OnHit));
    }

    protected override IEnumerator AttackAnimation(AttackCallback? attackCallback)
    {
        Origin.animator.enabled = false;
        SpriteRenderer spriteRenderer = Origin.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationFrame[0];
        yield return new WaitForSeconds(0.10f);
        spriteRenderer.sprite = animationFrame[1];
        yield return new WaitForSeconds(0.16f);
        Vector3 originalPosition = Origin.myTransform.position;
        Origin.myTransform.position = originalPosition + new Vector3(0, 1.5f, 0);
        spriteRenderer.sprite = animationFrame[2];
        yield return new WaitForSeconds(0.28f);
        spriteRenderer.sprite = animationFrame[3];
        Origin.myTransform.position = originalPosition;
        attackCallback?.Invoke();
        yield return new WaitForSeconds(0.20f);
        spriteRenderer.sprite = animationFrame[4];
        Origin.animator.enabled = true;
    }
}

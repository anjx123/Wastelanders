using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSmash : SlimeAttacks
{
    [SerializeField]
    private List<Sprite> animationFrame = new();

    protected bool original = true;

    public override void ExecuteActionEffect()
    {

    }

   

    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;
        Speed = 2;
        Block = 2;

        myName = "StackSmash";
        description = "If this attack is unstaggered, attack again";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;

    }
    public override void ApplyEffect()
    {
        DupInit();

        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

    //@Author: Anrui. Called by ActionClass.OnHit() 
    public override void CardIsUnstaggered()
    {
        if (original)
        {
            List<PlayerClass> players = CombatManager.Instance.GetPlayers();
            SlimeStack origin = (SlimeStack)Origin;
            List<GameObject> dupActions = origin.dupActions;
            // NOTE HERE THAT THIS RELIES ON KNOWLEDGE OF 0 i.e. place in the dupActions; could implement as a HashMap
            StackSmashDuplicate a = Instantiate(dupActions[0]).GetComponent<StackSmashDuplicate>(); // Instantiate a prefab; why isn't this superimposed?
            a.Origin = origin;
            a.Target = players[Random.Range(0, players.Count - 1)];
            a.original = false;
            BattleQueue.BattleQueueInstance.InsertDupEnemyAction(a);
        }
        // Muhammad

        // base.OnHit(); NOT NEEDED HERE AS ATTACKANIMATION causes the damage 
        // Muhammad end

    }
    public override void OnHit()
    {
        StartCoroutine(AttackAnimation());
    }

    public IEnumerator AttackAnimation()
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
        base.OnHit();
        yield return new WaitForSeconds(0.20f);
        spriteRenderer.sprite = animationFrame[4];
        Origin.animator.enabled = true;
    }
}

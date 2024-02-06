using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSmash : SlimeAttacks
{
    [SerializeField]
    private List<Sprite> animationFrame = new();
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
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
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }
    public override void ApplyEffect()
    {
        DupInit();

        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

    public override IEnumerator OnHit()
    {
        //Origin.AttackAnimation("IsStackSmashing");
        yield return StartCoroutine(AttackAnimation());
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
        Coroutine coroutine = StartCoroutine(base.OnHit());
        yield return new WaitForSeconds(0.20f);
        spriteRenderer.sprite = animationFrame[4];
        Origin.animator.enabled = true;
        yield return coroutine;
    }
}

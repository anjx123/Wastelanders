using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Pound : SlimeAttacks
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
        upperBound = 5;
        Speed = 2;
        Block = 2;

        myName = "Pound";
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
        yield return StartCoroutine(AttackAnimation());
        //Origin.AttackAnimation("IsPounding");
    }


    public IEnumerator AttackAnimation()
    {
        Vector3 originalPosition = Origin.myTransform.position;
        Origin.animator.enabled = false;
        SpriteRenderer spriteRenderer = Origin.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationFrame[0];
        Origin.myTransform.position = originalPosition;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.sprite = animationFrame[1];
        Coroutine coroutine = StartCoroutine(base.OnHit());
        yield return new WaitForSeconds(0.010f);
        spriteRenderer.sprite = animationFrame[2];
        yield return new WaitForSeconds(0.32f);
        spriteRenderer.sprite = animationFrame[3];
        yield return new WaitForSeconds(0.20f);
        spriteRenderer.sprite = animationFrame[4];
        Origin.animator.enabled = true;
        Origin.myTransform.position = originalPosition;
        yield return coroutine;
    }
}

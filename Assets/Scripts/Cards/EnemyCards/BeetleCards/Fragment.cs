using UnityEngine;


public class Fragment : ActionClass
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 3;
        
        Speed = 5;

        description = "If this card hits a player, gain +1 resonate";

        myName = "Fragment";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }



    public override void OnHit()
    {
        base.OnHit();
        if (Target is PlayerClass) {
            Origin.AddStacks(Resonate.buffName, 1);
        }
    }
}

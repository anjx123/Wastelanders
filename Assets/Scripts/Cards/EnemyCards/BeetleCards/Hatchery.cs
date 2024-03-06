using UnityEngine;


public class Hatchery : ActionClass
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;
        
        Speed = 1;

        description = "Spend +2 resonate to play this card. If this card is unstaggered, spawn 1 random beetle.";

        myName = "Hatchery";
        CardType = CardType.Defense;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
    }



    public override void OnHit()
    {
        // TODO: spawn beetle
    }
}

using UnityEngine;

public class Brace : FistCards
{

#nullable enable
    Brace? activeDuplicateInstance = null;
    bool originalCopy = true;

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 2;
        Speed = 3;

        myName = "Brace";
        description = "Block once, then block again!";
        CardType = CardType.Defense;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<Brace>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            if (activeDuplicateInstance.Origin is PlayerClass)
            { 
                BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!); //Gonna need a rewrite on this
            } else
            {
                BattleQueue.BattleQueueInstance.InsertDupEnemyAction(activeDuplicateInstance!);
            }
        }
        base.ApplyEffect();
    }
}

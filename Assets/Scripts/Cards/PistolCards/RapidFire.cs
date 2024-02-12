using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : PistolCards
{

    
    public override void ExecuteActionEffect()
    {
        
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 2;
        Block = 2;
        Damage = 3;
        description = "Attack, Lose 1 accuracy, then make this attack again.";
        CardType = CardType.MeleeAttack;
        myName = "RapidFire";
        myDescription = "Attack, Lose 1 Accuracy, Then Make This Attack Again";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }

    public override void OnHit()
    {
        base.OnHit();
        Origin.ReduceStacks(Accuracy.buffName, 1); // Reduce Accuracy by 1
        ApplyEffect(); // Reinitializes roll values
        RollDice(); // Calculates new Roll
        base.OnHit(); // Makes attack again
    }



}

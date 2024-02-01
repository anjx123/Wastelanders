using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusedStrike : StaffCards
{
    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        CardType = CardType.MeleeAttack;
        myName = "FocusedStrike";
        myDescription = "Gain 1 Focus, Then Attack";
        lowerBound = 2;
        upperBound = 2;
        Speed = 2;
       
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card, maybe refac into ActionClass?
        OriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect()
    {     


        Origin.AddStacks(Focus.buffName, 1);
        base.ApplyEffect();
    }

}

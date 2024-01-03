using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.UI.Image;

public class Headshot : PistolCards
{
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Start()
    {
        lowerBound = 1;
        upperBound = 8;
        base.Start();
        Speed = 1;
        Block = 2;
        Damage = 3;

        myName = "Headshot";
        CardType = CardType.RangedAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;



    }

    // Update is called once per frame
    void Update()
    {
    }
    public override void ApplyEffect()
    {

        DupInit();
        Debug.Log(Accuracy.buffName + Origin.getName() + duplicateCard.rollCeiling);


        Origin.AddStacks(ref duplicateCard, Accuracy.buffName);
        Origin.ApplyBuffsToCard(ref duplicateCard, Accuracy.buffName);
        Origin.ApplyBuffsToCard(ref duplicateCard, Focus.buffName);

    }

    public override void RollDice()
    {
        base.RollDice();
        duplicateCard.actualRoll += Origin.GetBuffStacks(Accuracy.buffName);
    }
}

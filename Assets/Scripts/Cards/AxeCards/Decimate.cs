using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Decimate : AxeCards
{
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 6;
        Speed = 2;

        myName = "Decimate";
        description = "On hit, double the amount of wounds on the target. Then add an aditional wound.";
        evolutionDescription = "Double 10 wounds.";
        MaxEvolutionProgress = 10;

        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material;
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.MeleeAttack;
    }

    public override void OnHit()
    {
        base.OnHit();
        CurrentEvolutionProgress += Target.GetBuffStacks(Wound.buffName);
        Debug.Log("My current evolution progress is: " + CurrentEvolutionProgress + " out of " + MaxEvolutionProgress);
        Target.AddStacks(Wound.buffName, Target.GetBuffStacks(Wound.buffName));
        Target.AddStacks(Wound.buffName, 1);
    }
}

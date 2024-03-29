using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SharpenedDefence : AxeCards
{
    public override void Initialize()
    {
    base.Initialize(); // Implementation based on some other card // TOD0: works as intended; if not then set CardType manually in each derived class and remove equation from base
    lowerBound = 2;
    upperBound = 4;
    Speed = 3;
    CardType = CardType.Defense;    

    myName = "SharpenedDefence";
    description = "If this card is unstaggered, inflict 2 wound.";
    Renderer renderer = GetComponent<Renderer>();
    ogMaterial = renderer.material;
    OriginalPosition = transform.position;
    }

    public override void CardIsUnstaggered()
    {
        Target.AddStacks(Wound.buffName, 2);
        base.CardIsUnstaggered();
    }
}

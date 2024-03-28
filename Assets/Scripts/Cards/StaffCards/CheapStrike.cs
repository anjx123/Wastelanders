using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheapStrike : StaffCards
{
    public override void OnCardStagger()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 1;

        Speed = 1;


        myName = "Cheap Strike";
        description = "On Hit, gain 1 Focus next turn";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        CardType = CardType.MeleeAttack;
        base.Initialize();


    }

    public override void OnHit()
    {
        base.OnHit();
        CombatManager.OnGameStateChanged += AddFocus;
        void AddFocus(GameState gameState)
        {
            if (gameState == GameState.SELECTION)
            {
                CombatManager.OnGameStateChanged -= AddFocus;
                Origin.AddStacks(Focus.buffName, 1);
            }
        }
    }
}

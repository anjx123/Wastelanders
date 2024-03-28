using UnityEngine;
using static StatusEffect;

public class SteadiedShot : PistolCards
{

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;
        Speed = 4;

        myName = "Steadied Shot";
        description = "When played, do not lose accuracy when you get hit this round.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.Defense;
    }

    public override void ApplyEffect()
    {
        StatusEffectDelegate originalHandler = Origin.SetBuffsOnHitHandler(Accuracy.buffName, () => { });
        CombatManager.OnGameStateChanged += ResetBuffHandler;
        void ResetBuffHandler(GameState gameState)
        {
            if (gameState == GameState.SELECTION)
            {
                CombatManager.OnGameStateChanged -= ResetBuffHandler;
                Origin.SetBuffsOnHitHandler(Accuracy.buffName, originalHandler);
            }
        }
    }
}

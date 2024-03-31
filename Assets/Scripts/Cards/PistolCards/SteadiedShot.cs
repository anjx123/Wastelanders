using UnityEngine;
using static StatusEffect;

public class SteadiedShot : PistolCards
{
    private CombatManager.GameStateChangedHandler resetBuffHandler;
    public void OnDestroy()
    {
        if (resetBuffHandler != null)
        {
            CombatManager.OnGameStateChanged -= resetBuffHandler;
        }
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 3;
        Speed = 4;

        myName = "Steadied Shot";
        description = "When played, do not lose Accuracy when you get hit this round.";
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
        CardType = CardType.Defense;
    }

    public override void ApplyEffect()
    {
        StatusEffectModifyValueDelegate originalHandler = Origin.SetBuffsOnHitHandler(Accuracy.buffName, (ref int damage) => { });
        resetBuffHandler = ResetBuffHandler;
        CombatManager.OnGameStateChanged += ResetBuffHandler;
        void ResetBuffHandler(GameState gameState)
        {
            if (gameState != GameState.FIGHTING)
            {
                CombatManager.OnGameStateChanged -= ResetBuffHandler;
                Origin.SetBuffsOnHitHandler(Accuracy.buffName, originalHandler);
            }
        }
        base.ApplyEffect();
    }
}

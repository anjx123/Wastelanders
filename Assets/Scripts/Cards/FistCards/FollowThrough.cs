using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThrough : FistCards
{
#nullable enable
    private FollowThrough? activeDuplicateInstance = null;
    private bool originalCopy = true;
    CombatManager.GameStateChangedHandler? GameStateChangedHandler;
    private void OnDestroy()
    {
        if (GameStateChangedHandler != null)
        {
            CombatManager.OnGameStateChanged -= GameStateChangedHandler;
        }
    }
    public override void Initialize()
    {

        CardType = CardType.MeleeAttack;
        myName = "Follow Through";
        description = "If unstaggered, make this attack against the target again everytime they are staggered this turn";
        lowerBound = 1;
        upperBound = 2;
        Speed = 5;

        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {

        if (originalCopy)
        {
            if (activeDuplicateInstance == null)
            {
                activeDuplicateInstance = Instantiate(this.GetComponent<FollowThrough>());
                activeDuplicateInstance.originalCopy = false;
                activeDuplicateInstance.transform.position = new Vector3(-10, 10, 10);
            }
            activeDuplicateInstance.Origin = Origin;
            activeDuplicateInstance.Target = Target;
            void MakeAnotherAttack(int damage)
            {
                if (damage > 0)
                {
                    BattleQueue.BattleQueueInstance.InsertDupPlayerAction(activeDuplicateInstance!);
                }
            }

            Target.EntityTookDamage += MakeAnotherAttack;
            CombatManager.OnGameStateChanged += ResetBuffHandler;
            void ResetBuffHandler(GameState gameState)
            {
                if (gameState != GameState.FIGHTING)
                {
                    CombatManager.OnGameStateChanged -= ResetBuffHandler;
                    Target.EntityTookDamage -= MakeAnotherAttack;
                }
            }

            GameStateChangedHandler = ResetBuffHandler;
        }


        base.CardIsUnstaggered();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneDialogue : DialogueClasses
{
    protected override void GameStateChange(GameState gameState)
    {
        if (gameState == GameState.GAME_START)
        {
            CombatManager.Instance.GameState = GameState.SELECTION;
        }
    }

}

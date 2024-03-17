using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classes that have ultimate power over a scene and can script events to occur. Please ensure that no references to Dialogue Classes can ever be held as they will be mixed and matched across scenes
public abstract class DialogueClasses : MonoBehaviour
{
    private void Awake()
    {
        CombatManager.OnGameStateChanged += GameStateChange;
    }

    private void OnDestroy()
    {
        CombatManager.OnGameStateChanged -= GameStateChange;
    }


    protected abstract void GameStateChange(GameState gameState);
}

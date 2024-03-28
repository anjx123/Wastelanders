using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartDequeuing : MonoBehaviour
{
    public Sprite buttonUp;
    public Sprite buttonDown;

    public SpriteRenderer spriteRenderer;

    private void OnMouseEnter()
    {
        if (!PauseMenu.IsPaused)
        {
            spriteRenderer.sprite = buttonDown;
        }
    }

    private void OnMouseExit()
    {
        if (!PauseMenu.IsPaused)
        {
            spriteRenderer.sprite = buttonUp;
        }
    }
    // BQ reference not needed. 

    void OnMouseDown()
    {
        if (!PauseMenu.IsPaused)
        {
            BattleQueue.BattleQueueInstance.BeginDequeue();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UI_Toolkit;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartDequeuing : MonoBehaviour
{
    public Sprite buttonUp;
    public Sprite buttonDown;

    public SpriteRenderer spriteRenderer;

    private void OnMouseEnter()
    {
        if (!PauseMenuV2.IsPaused)
        {
            spriteRenderer.sprite = buttonDown;
        }
    }

    private void OnMouseExit()
    {
        if (!PauseMenuV2.IsPaused)
        {
            spriteRenderer.sprite = buttonUp;
        }
    }
    // BQ reference not needed. 

    void OnMouseDown()
    {
        if (!PauseMenuV2.IsPaused)
        {
            BattleQueue.BattleQueueInstance.BeginDequeue();
        }
    }
}

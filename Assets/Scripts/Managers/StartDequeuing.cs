using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDequeuing : MonoBehaviour
{
    public Sprite buttonUp;
    public Sprite buttonDown;

    public SpriteRenderer spriteRenderer;

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = buttonDown;
    }

    private void OnMouseExit()
    {
        spriteRenderer.sprite = buttonUp;
    }
    // BQ reference not needed. 

    void OnMouseDown()
    {
        BattleQueue.BattleQueueInstance.BeginDequeue();
    }
}

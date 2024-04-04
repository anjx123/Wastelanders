using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleQueueIcons : DisplayableClass
{
    [SerializeField] SpriteRenderer targetRenderer;

    public override void OnMouseDown()
    {
        if (CombatManager.Instance.CanHighlight())
        {
            ShowCard();
        }
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && ActionClass.Origin is PlayerClass && CombatManager.Instance.CanHighlight())
        {
            DeleteFromBQ();
        }
    }

    private void DeleteFromBQ()
    {
        if (CombatManager.Instance.CanHighlight())
        {
            DeHighlightTarget();
            BattleQueue.BattleQueueInstance.DeletePlayerAction(ActionClass);
            HighlightManager.Instance.currentHighlightedAction = null;
            HighlightManager.Instance.currentHighlightedEnemyEntity = null;
        }  
    }


    public override void OnMouseEnter()
    {
        // Increase the size of the Combat UI to indicate it's clickable
        if (CombatManager.Instance.CanHighlight() && !PauseMenu.IsPaused)
        {
            Vector3 scale = transform.localScale;
            scale *= 1.25f;
            transform.localScale = scale;
            HighlightTarget();
        }
    }

    public override void OnMouseExit()
    {
        // Reset the size when the mouse is no longer over the Combat UI
        Vector3 scale = new Vector3(20, 20, (float)1.25);
        transform.localScale = scale;
        DeHighlightTarget();
    }

    public void RenderBQIcon(ActionClass ac)
    {
        ActionClass = ac;
        targetRenderer.sprite = ac.Target.icon;
        GetComponent<SpriteRenderer>().sprite = ac.GetIcon();
    }
}

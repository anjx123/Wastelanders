using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleQueueIcons : DisplayableClass
{
    public override void OnMouseDown()
    {
        if (actionClass.Origin is PlayerClass) {
            DeleteFromBQ();
        } else {
            if (CombatManager.Instance.CanHighlight())
            {
                ShowCard();
            }
        }
    }

    private void DeleteFromBQ()
    {
        if (CombatManager.Instance.CanHighlight())
        {
            DeHighlightTarget();
            BattleQueue.BattleQueueInstance.DeletePlayerAction(actionClass);
        }  
    }


    public override void OnMouseEnter()
    {
        // Increase the size of the Combat UI to indicate it's clickable
        if (CombatManager.Instance.CanHighlight())
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

    public void renderBQIcon(ActionClass ac)
    {
        actionClass = ac;
        GetComponent<SpriteRenderer>().sprite = ac.GetIcon();
    }
}

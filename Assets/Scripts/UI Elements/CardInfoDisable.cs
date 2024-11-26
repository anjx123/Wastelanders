using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInfoDisable : MonoBehaviour
{
    public void OnMouseDown()
    {
        CombatCardDisplayManager.Instance.HideCardOnClick();
    }
}

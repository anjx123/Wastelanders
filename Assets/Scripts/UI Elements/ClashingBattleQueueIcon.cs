using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashingBattleQueueIcon : MonoBehaviour
{
    [SerializeField]
    private BattleQueueIcons leftClashingAction;
    [SerializeField]
    private BattleQueueIcons rightClashingAction;

    public void renderClashingIcons(ActionClass leftClashingItem,  ActionClass rightClashingItem)
    {
        leftClashingAction.RenderBQIcon(leftClashingItem);
        rightClashingAction.RenderBQIcon(rightClashingItem);
    }
}

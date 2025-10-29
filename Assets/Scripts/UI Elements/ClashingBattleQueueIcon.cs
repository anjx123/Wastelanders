using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashingBattleQueueIcon : MonoBehaviour
{
    [SerializeField]
    private BattleQueueIcons leftClashingAction;
    [SerializeField]
    private BattleQueueIcons rightClashingAction;
    [SerializeField]
    private SpriteRenderer swordsIcon;

    public void Start()
    {
        swordsIcon.sortingLayerName = CombatFadeScreenHandler.Instance.FADE_SORTING_LAYER;
        swordsIcon.sortingOrder = CombatFadeScreenHandler.Instance.FADE_SORTING_ORDER + 6;
    }
    public void renderClashingIcons(ActionClass leftClashingItem,  ActionClass rightClashingItem)
    {
        leftClashingAction.RenderBQIcon(leftClashingItem);
        rightClashingAction.RenderBQIcon(rightClashingItem);
    }
}

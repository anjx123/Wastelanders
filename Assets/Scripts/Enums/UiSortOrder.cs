/*
 * Enum that decides the sorting order of all UI elements. 
 */
public enum UISortOrder
{
    Base,
    Hudv2,
    DialogueBox,
    WarningPopup,
    CombatIntro,
    PauseMenu,
    FadeScreen
}

public static class UiSortOrderHelpers
{
    public static int GetOrder(this UISortOrder order) => (int) order;
} 

public enum UISortOrder
{
    Base,
    Hudv2,
    WarningPopup,
    CombatIntro,
    PauseMenu,
    FadeScreen
}

public static class UiSortOrderHelpers
{
    public static int GetOrder(this UISortOrder order) => (int) order;
} 
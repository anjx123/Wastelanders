using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Toolkit.UI_Elements
{
    public class CardV2 : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<CardV2>
        {
        }

        public void WithAttrsFromActionClass(ActionClass ac)
        {
            WithAttrs(ac.GetIcon(), ac.GetName(), ac.Speed.ToString(), FormatStats(ac.GetRolledStats()));

            var icon = this.Q<VisualElement>("img-card-icon");
            icon.ClearClassList();
            icon.AddToClassList(ac.CardType == CardType.Defense ? "stat-icon-def" : "stat-icon-atk");

            var back = this.Q<VisualElement>("img-card-back");
            back.ClearClassList();
            back.AddToClassList($"card-back-{ac switch { AxeCards => "a", FistCards => "f", PistolCards => "p", StaffCards => "s", _ => "e" }}");
        }

        public void BindActionClassCardState(ActionClass ac)
        {
            ac.OnCardStateChanged += WithState;
            WithState(ac.cardState);
            RegisterCallback<DetachFromPanelEvent>(_ => ac.OnCardStateChanged -= WithState);
        }

        public void BindActionClassCallbacks(ActionClass ac)
        {
            RegisterCallback<MouseDownEvent>(_ => ac.OnMouseDown());
            RegisterCallback<MouseEnterEvent>(_ => ac.OnMouseEnter());
            RegisterCallback<MouseLeaveEvent>(_ => ac.OnMouseExit());
        }

        private void WithAttrs(
            Sprite fg = null,
            string tt = null,
            string sp = null,
            string st = null)
        {
            if (fg) this.Q<VisualElement>("img-card-icon").style.backgroundImage = new StyleBackground(fg);
            if (tt != null) this.Q<Label>("txt-title").text = tt;
            if (sp != null) this.Q<Label>("txt-speed").text = sp;
            if (st != null) this.Q<Label>("txt-stats").text = st;
        }

        private void WithState(ActionClass.CardState state)
        {
            ClearClassList();
            AddToClassList($"card-state-{state switch { ActionClass.CardState.CANT_PLAY => "1", ActionClass.CardState.CLICKED_STATE => "2", _ => "0" }}");
        }

        private static string FormatStats(ActionClass.RolledStats stats) => $"<color=#{stats.FloorBuffs switch { > 0 => "00FF", < 0 => "FF00", _ => "0000" }}00>{stats.RollFloor}</color> - <color=#{stats.CeilingBuffs switch { > 0 => "00FF", < 0 => "FF00", _ => "0000" }}00>{stats.RollCeiling}</color>";
    }
}
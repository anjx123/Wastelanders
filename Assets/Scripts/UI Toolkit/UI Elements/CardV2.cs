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
            WithAttrs(ac.GetIcon(), ac.Speed.ToString(), ac.GetName(), FormatStats(ac.GetRolledStats()));

            var back = this.Q<VisualElement>("image-bg");
            back.ClearClassList();
            back.AddToClassList(CardBackFor(ac));

            var icon = this.Q<VisualElement>("image-ic");
            icon.ClearClassList();
            icon.AddToClassList(ac.CardType == CardType.Defense ? "dynamic-ic-def" : "dynamic-ic-atk");
        }

        private void WithAttrs(
            Sprite fg = null,
            string sp = null,
            string tt = null,
            string st = null)
        {
            if (fg) this.Q<VisualElement>("image-fg").style.backgroundImage = new StyleBackground(fg);
            if (sp != null) this.Q<Label>("label-sp").text = sp;
            if (tt != null) this.Q<Label>("label-tt").text = tt;
            if (st != null) this.Q<Label>("label-st").text = st;
        }

        private static string CardBackFor(ActionClass ac)
        {
            return
                $"dynamic-bg-{ac switch { AxeCards => "axe", FistCards => "fist", PistolCards => "pistol", StaffCards => "staff", _ => "enemy" }}";
        }

        private static string FormatStats(ActionClass.RolledStats stats)
        {
            return
                $"<color=#{stats.FloorBuffs switch { > 0 => "00FF", < 0 => "FF00", _ => "0000" }}00>{stats.RollFloor}</color> - <color=#{stats.CeilingBuffs switch { > 0 => "00FF", < 0 => "FF00", _ => "0000" }}00>{stats.RollCeiling}</color>";
        }
    }
}
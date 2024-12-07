
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuffExplainer : MonoBehaviour
{
    [SerializeField] SpriteRenderer buffIcon;
    [SerializeField] TextMeshPro explanationTitleField;
    [SerializeField] TextMeshPro explanationTextField;
    [SerializeField] public List<WeaponExplanation> explanationText;
#nullable enable
    private StatusEffect? currentEffect;

    public void RenderExplanationForBuff(CardDatabase.WeaponType weaponType)
    {
        explanationTextField.text = explanationText.FirstOrDefault(tuple => tuple.WeaponType == weaponType)?.ExplanationText;
        explanationTitleField.text = explanationText.FirstOrDefault(tuple => tuple.WeaponType == weaponType)?.ExplanationTitle;
        currentEffect = weaponType switch
        {
            CardDatabase.WeaponType.STAFF => new Flow(),
            CardDatabase.WeaponType.PISTOL => new Accuracy(),
            CardDatabase.WeaponType.FIST => null,
            CardDatabase.WeaponType.AXE => new Wound(),
            _ => null
        };

        buffIcon.sprite = currentEffect?.GetIcon();
    }


    [System.Serializable]
    public class WeaponExplanation
    {
        [field: SerializeField] public CardDatabase.WeaponType WeaponType { get; set; }
        [field: SerializeField] public string ExplanationTitle { get; set; } = "";
        [field: SerializeField] public string ExplanationText { get; set; } = "";
    }
}

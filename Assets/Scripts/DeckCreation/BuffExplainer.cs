
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuffExplainer : MonoBehaviour
{
    [SerializeField] SpriteRenderer buffIcon;
    [SerializeField] TextMeshPro explanationTextField;
    [SerializeField] List<WeaponExplanation> explanationText;
#nullable enable
    private StatusEffect? currentEffect;

    public void RenderExplanationForBuff(CardDatabase.WeaponType weaponType)
    {
        explanationTextField.text = explanationText.FirstOrDefault(tuple => tuple.WeaponType == weaponType).ExplanationText;
        switch (weaponType)
        {
            case CardDatabase.WeaponType.STAFF:
                currentEffect = new Flow();
                break;
            case CardDatabase.WeaponType.PISTOL:
                currentEffect = new Accuracy();
                break;
            case CardDatabase.WeaponType.FIST:
                currentEffect = null;
                break;
            case CardDatabase.WeaponType.AXE:
                currentEffect = new Wound();
                break;
        }

        buffIcon.sprite = currentEffect?.GetIcon();
    }


    [System.Serializable]
    private class WeaponExplanation
    {
        [field: SerializeField] public CardDatabase.WeaponType WeaponType { get; set; }
        [field: SerializeField] public string ExplanationText { get; set; } = "";
    }
}

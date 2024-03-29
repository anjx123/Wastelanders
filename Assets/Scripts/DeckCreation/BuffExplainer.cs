using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class BuffExplainer : MonoBehaviour
{
    [SerializeField] SpriteRenderer buffIcon;
    [SerializeField] TextMeshPro explanationTextField;
    [SerializeField] List<SerializableTuple<CardDatabase.WeaponType, string>> explanationText;
#nullable enable
    private StatusEffect? currentEffect;

    public void RenderExplanationForBuff(CardDatabase.WeaponType weaponType)
    {
        explanationTextField.text = explanationText.FirstOrDefault(tuple => tuple.Item1 == weaponType).Item2;
        switch (weaponType)
        {
            case CardDatabase.WeaponType.STAFF:
                currentEffect = new Focus();
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
}

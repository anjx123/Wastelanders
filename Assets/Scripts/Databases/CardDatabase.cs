using UnityEngine;
using System.Collections.Generic;
using TMPro;


/*
 * @author Anrui
 Represents All the player cards in the game.
 All Cards loaded in the corresponding scriptable object will be loaded during deck selection
 */
[CreateAssetMenu(fileName = "NewCardDatabase", menuName = "Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<StaffCards> staffCards;
    public List<PistolCards> pistolCards;
    
    public List<ActionClass> GetCardsByType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.STAFF: return new List<ActionClass>(staffCards);
            case WeaponType.PISTOL: return new List<ActionClass>(pistolCards);
            case WeaponType.AXE: return new List<ActionClass>(pistolCards);
            case WeaponType.FIST: return new List<ActionClass>(pistolCards);
            default:
                Debug.LogWarning("Weapon Type is currently unsupported");
                return null;
        }
    }

    public enum WeaponType
    {
        STAFF,
        PISTOL,
        FIST,
        AXE,
    }
}

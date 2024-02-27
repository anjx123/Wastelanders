using UnityEngine;
using System.Collections.Generic;


/*
 * @author Anrui
 Represents All the player cards in the game.
 All Cards loaded in the corresponding scriptable object will be loaded during deck selection
 */
[CreateAssetMenu(fileName = "NewCardDatabase", menuName = "Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<StaffCards> staffCards = new();
    public List<PistolCards> pistolCards = new();

    public enum WeaponType
    {
        STAFF,
        PISTOL,
        FIST,
        AXE,
    }
}

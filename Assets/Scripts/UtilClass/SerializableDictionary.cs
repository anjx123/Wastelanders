using System;
using System.Collections.Generic;
using UnityEngine;

// Maps a weapon type to a list of the class names of the cards that a player holds in their deck.
// We now use strings instead of action Class prefab references because strings are serializable.
[System.Serializable]
public class SerializableWeaponListEntry
{
    public CardDatabase.WeaponType key;
    public List<string> value;
}
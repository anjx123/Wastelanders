using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableWeaponListEntry
{
    public CardDatabase.WeaponType key;
    public List<ActionClass> value;
}
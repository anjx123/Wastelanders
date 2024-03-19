using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that holds the information of all players like level, weapon proficiency, decks. Can be edited in the unity editor
 *  */
[CreateAssetMenu(fileName = "New Player Database", menuName = "Player Database")]
public class PlayerDatabase : ScriptableObject
{
    public PlayerData JackieData;
    public PlayerData IvesData;

    #nullable enable //Turns on pedantic null checks 
    public List<ActionClass> GetDeck(string myName)
    {
        PlayerData playerData;
        if (myName == "Jackie") {
            playerData = JackieData;
        } else if (myName == "Ives") {
            playerData = IvesData;
        } else {
            return new List<ActionClass>();
        }

        List<ActionClass> combinedDeck = new();

        foreach (var entry in playerData.playerDeck) {
            if (playerData.weapons.Contains(entry.key)) {
                combinedDeck.AddRange(entry.value);
            }
        }

        return combinedDeck;
    }

    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public List<SerializableTuple<CardDatabase.WeaponType, int>> playerWeaponProficiency = new();
        [SerializeField]
        public List<SerializableWeaponListEntry> playerDeck = new();
        public List<CardDatabase.WeaponType> weapons = new();

        private (CardDatabase.WeaponType, List<ActionClass>)? _deck1 = null;
        private (CardDatabase.WeaponType, List<ActionClass>)? _deck2 = null;

        public (CardDatabase.WeaponType, List<ActionClass>)? Deck1
        {
            get { return _deck1; }
            set { _deck1 = value; }
        }

        public (CardDatabase.WeaponType, List<ActionClass>)? Deck2
        {
            get { return _deck2; }
            set { _deck2 = value; }
        }

        //Gets the combination of both smaller decks
        public List<ActionClass> GetCombinedDeck()
        {
            List<ActionClass> combinedDeck = new List<ActionClass>();

            if (Deck1?.Item2 != null)
            {
                combinedDeck.AddRange(Deck1?.Item2);
            }
            if (Deck2?.Item2 != null)
            {
                combinedDeck.AddRange(Deck2?.Item2);
            }

            return combinedDeck;
        }

        //Gets the corresponding deck by a weapon type or null.
        public List<ActionClass>? GetDeckByWeaponType(CardDatabase.WeaponType weaponType) 
        {
            if (weaponType == Deck1?.Item1)
            {
                return Deck1?.Item2;
            }

            if (weaponType == Deck2?.Item1)
            {
                return Deck2?.Item2;
            }

            return null;
        }


    }
}



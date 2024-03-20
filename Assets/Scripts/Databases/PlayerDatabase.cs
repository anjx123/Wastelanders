using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerDatabase;

/*
 * Class that holds the information of all players like level, weapon proficiency, decks. Can be edited in the unity editor
 *  */
[CreateAssetMenu(fileName = "New Player Database", menuName = "Player Database")]
public class PlayerDatabase : ScriptableObject
{
    public PlayerData JackieData;
    public PlayerData IvesData;

    public PlayerData GetDataByPlayerName(PlayerName player)
    {
        switch (player)
        {
            case PlayerName.JACKIE:
                return JackieData;
            case PlayerName.IVES:
                return IvesData;
            default:
                throw new System.Exception("No Valid Database for that player name" + player);
        }
    }

    public List<ActionClass> GetDeckByPlayerName(PlayerName player)
    {
        switch (player)
        {
            case PlayerName.JACKIE:
                return JackieData.GetCombinedDeck();
            case PlayerName.IVES:
                return IvesData.GetCombinedDeck();
            default:
                throw new System.Exception("No Valid Database for that player name" + player);
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string name = "";
        public List<SerializableTuple<CardDatabase.WeaponType, int>> playerWeaponProficiency = new();
        public List<CardDatabase.WeaponType> selectedWeapons = new();
        public List<SerializableWeaponListEntry> playerDeck = new();


        //Gets the combination of both smaller decks
        public List<ActionClass> GetCombinedDeck()
        {
            List<ActionClass> combinedDeck = new List<ActionClass>();

            foreach (var entry in playerDeck)
            {
                if (selectedWeapons.Contains(entry.key))
                {
                    combinedDeck.AddRange(entry.value);
                }
            }

            return combinedDeck;
        }

        //Like a dictionary, gets the player deck by the weapon type, if not contained, return a empty list
        public List<ActionClass> GetDeckByWeaponType(CardDatabase.WeaponType weaponType)
        {
            foreach (var entry in playerDeck)
            {
                if (entry.key == weaponType)
                {
                    return entry.value;
                }
            }

            return new List<ActionClass>();
        }
    }

    public enum PlayerName
    {
        JACKIE,
        IVES,
    }
}



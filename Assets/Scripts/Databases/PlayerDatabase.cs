using System;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using static PlayerDatabase;

/*
 * Class that holds the information of all players like level, weapon proficiency, decks. Can be edited in the unity editor
 *  */
[CreateAssetMenu(fileName = "New Player Database", menuName = "Player Database")]
public class PlayerDatabase : ScriptableObject, IBind<PlayerInformation>
{
    public PlayerData JackieData;
    public PlayerData IvesData;

    public SerializableGuid Id { get; set;}

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

    public List<string> GetDeckByPlayerName(PlayerName player)
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

    public void Bind(PlayerInformation data)
    {
        JackieData = data.JackieData;
        IvesData = data.IvesData;
    }

    [System.Serializable]
    public class PlayerData
    {
        public string name = ""; 
        public List<SerializableTuple<CardDatabase.WeaponType, SerializableTuple<int, int>>> playerWeaponProficiency = new(); //left int is current points taken up, right is MAX proficiency points
        public List<CardDatabase.WeaponType> selectedWeapons = new();
        public List<SerializableWeaponListEntry> playerDeck = new();


        //Gets the combination of both smaller decks
        public List<string> GetCombinedDeck()
        {
            List<string> combinedDeck = new();

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
        public List<string> GetDeckByWeaponType(CardDatabase.WeaponType weaponType)
        {
            foreach (var entry in playerDeck)
            {
                if (entry.key == weaponType)
                {
                    return entry.value;
                }
            }

            return new List<string>();
        }
    }

    public enum PlayerName
    {
        JACKIE,
        IVES,
    }
}


[System.Serializable]
public class PlayerInformation : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public PlayerData JackieData { get; set; }
    [field: SerializeField] public PlayerData IvesData { get; set; }    

    public PlayerInformation(PlayerData jackieData, PlayerData ivesData)
    {
        JackieData = jackieData;
        IvesData = ivesData;
    }
}



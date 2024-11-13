using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using WeaponDeckSerialization;
using static PlayerDatabase;

/*
 * Class that holds the information of all players like level, weapon proficiency, decks. Can be edited in the unity editor
 *  */
[CreateAssetMenu(fileName = "New Player Database", menuName = "Player Database")]
public class PlayerDatabase : ScriptableObject, IBind<PlayerInformation>
{
    public PlayerData JackieData;
    public PlayerData IvesData;

    public SerializableGuid Id { get; set; }

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

    public List<SerializableActionClassInfo> GetDeckByPlayerName(PlayerName player)
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
        public static readonly PlayerData JACKIE_DEFAULT = new
            (
            name: typeof(Jackie).Name,
            playerWeaponProficiency: new() {
                new WeaponProficiency(CardDatabase.WeaponType.PISTOL, 0, 12),
                new WeaponProficiency(CardDatabase.WeaponType.STAFF, 0, 12),
                new WeaponProficiency(CardDatabase.WeaponType.AXE, 0, 10),
                new WeaponProficiency(CardDatabase.WeaponType.FIST, 0, 10)
            },
            selectedWeapons: new() { CardDatabase.WeaponType.STAFF, CardDatabase.WeaponType.PISTOL },
            playerDeck: new()
            {
                new SerializableWeaponListEntry
                (
                    CardDatabase.WeaponType.PISTOL,
                    new()
                    {
                        new(typeof(IronSights).Name), new(typeof(Silencer).Name),
                        new(typeof(QuickDraw).Name), new(typeof(PistolWhip).Name),
                        new(typeof(HipFire).Name), new(typeof(RapidFire).Name)
                    }
                ),
                new SerializableWeaponListEntry
                (
                    CardDatabase.WeaponType.STAFF,
                    new()
                    {
                        new(typeof(CheapStrike).Name), new(typeof(FocusedStrike).Name),
                        new(typeof(Trip).Name), new(typeof(Flurry).Name),
                        new(typeof(Jab).Name), new(typeof(CounterStrike).Name)
                    }
                )
            }
            );

        public static readonly PlayerData IVES_DEFAULT = new(
            name: typeof(Ives).Name,
            playerWeaponProficiency: new() {
                new WeaponProficiency(CardDatabase.WeaponType.PISTOL, 0, 10),
                new WeaponProficiency(CardDatabase.WeaponType.STAFF, 0, 10),
                new WeaponProficiency(CardDatabase.WeaponType.AXE, 0, 12),
                new WeaponProficiency(CardDatabase.WeaponType.FIST, 0, 12)
            },
            selectedWeapons: new() { CardDatabase.WeaponType.AXE, CardDatabase.WeaponType.FIST },
            playerDeck: new()
            {
                new SerializableWeaponListEntry
                (
                    CardDatabase.WeaponType.AXE,
                    new() 
                    { 
                        new(typeof(Execute).Name), new(typeof(SharpenedDefence).Name),
                        new(typeof(Cleave).Name), new(typeof(Decimate).Name),
                        new(typeof(Mutilate).Name), new(typeof(Whirl).Name)
                    }
                ),
                new SerializableWeaponListEntry
                (
                    CardDatabase.WeaponType.FIST, 
                    new() 
                    { 
                        new(typeof(Brace).Name), new(typeof(LeftHook).Name), 
                        new(typeof(RightHook).Name), new(typeof(Pummel).Name), 
                        new(typeof(Haymaker).Name), new(typeof(FollowThrough).Name)
                    }
                 )
            }
            );

        public string name = "";
        public List<WeaponProficiency> playerWeaponProficiency = new();
        public List<CardDatabase.WeaponType> selectedWeapons = new();
        public List<SerializableWeaponListEntry> playerDeck = new();

        public PlayerData(string name, List<WeaponProficiency> playerWeaponProficiency, List<CardDatabase.WeaponType> selectedWeapons, List<SerializableWeaponListEntry> playerDeck)
        {
            this.name = name;
            this.playerWeaponProficiency = playerWeaponProficiency;
            this.selectedWeapons = selectedWeapons;
            this.playerDeck = playerDeck;
        }

        //Gets the combination of both smaller decks
        public List<SerializableActionClassInfo> GetCombinedDeck()
        {
            List<SerializableActionClassInfo> combinedDeck = new();

            foreach (var entry in playerDeck)
            {
                if (selectedWeapons.Contains(entry.weapon))
                {
                    combinedDeck.AddRange(entry.weaponDeck);
                }
            }

            return combinedDeck;
        }

        //Like a dictionary, gets the player weaponDeck by the weapon type, if not contained, return a empty list
        public List<SerializableActionClassInfo> GetDeckByWeaponType(CardDatabase.WeaponType weaponType)
        {
            foreach (var entry in playerDeck)
            {
                if (entry.weapon == weaponType)
                {
                    return entry.weaponDeck;
                }
            }

            return new List<SerializableActionClassInfo>();
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



using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;
using WeaponDeckSerialization;
using System.Security.Cryptography.X509Certificates;
using static ISubWeaponType;


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
    public List<FistCards> fistCards;
    public List<AxeCards> axeCards;
    public List<ActionClass> enemyCards;

    //Grabs the corresponding weaponDeck to the (@param weaponType)
    public List<ActionClass> GetCardsByType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.STAFF: return new List<ActionClass>(staffCards);
            case WeaponType.PISTOL: return new List<ActionClass>(pistolCards);
            case WeaponType.AXE: return new List<ActionClass>(axeCards);
            case WeaponType.FIST: return new List<ActionClass>(fistCards);
            case WeaponType.ENEMY: return new List<ActionClass>(enemyCards);
            default:
                Debug.LogWarning("Weapon Type is currently unsupported");
                return null;
        }
    }

    public List<ISubWeaponType> GetSubFoldersFor(WeaponType weaponType)
    {
        return weaponType switch
        {
            CardDatabase.WeaponType.ENEMY => PlayableEnemyWeapon.values,
            _ => new(),
        };
    }

    // Necessary to set the initial page that is loaded when we enter a subfolder
    public List<ActionClass> GetDefaultSubFolderData(WeaponType weaponType)
    {
        return GetSubFoldersFor(weaponType)[0].GetSubWeaponCards(this);
    }

    public List<ActionClass> GetAllCards()
    {
        List<ActionClass> allCards = new();
        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            allCards.AddRange(GetCardsByType(type));
        }
        return allCards;
    }

    public List<ActionData> GetDefaultActionDatas()
    {
        List<ActionData> allData = new();
        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            foreach (ActionClass action in GetCardsByType(type))
            {
                ActionData actionData = new ActionData();
                actionData.ActionClassName = action.GetType().Name;
                allData.Add(actionData);
            }
        }
        return allData;
    }


    // Converts a list of Action Class types to the actual prefab contained in this database. 
    public List<InstantiableActionClassInfo> GetPrefabInfoForDeck(List<SerializableActionClassInfo> tuples)
    {
        var instantiableCardInfos = tuples.Select(tuple => new InstantiableActionClassInfo(
            actionClass: GetAllCards().Find(actionClass => actionClass.GetType().Name == tuple.ActionClassName),
            isEvolved: tuple.IsEvolved)
        ).ToList();
        return instantiableCardInfos;
    }

    // For performance reasons, use this if you know the type
    public List<ActionClass> ConvertStringsToCards(WeaponType type, List<string> types)
    {
        return GetCardsByType(type).FindAll(actionClass => types.Contains(actionClass.GetType().Name));
    }


    public enum WeaponType
    {
        STAFF,
        PISTOL,
        FIST,
        AXE,
        ENEMY
    }
}

public interface IPlayableEnemyCard { }
public interface IPlayableBeetleCard : IPlayableEnemyCard { }
public interface IPlayableFrogCard : IPlayableEnemyCard{ }
public interface IPlayableSlimeCard : IPlayableEnemyCard { }
public interface IPlayableQueenCard : IPlayableEnemyCard { }
public interface IPlayablePrincessFrogCard : IPlayableEnemyCard { }

public interface ISubWeaponType
{
    public string Name { get; set; }
    public GetSubweaponCards GetSubWeaponCards { get; set; }

    public delegate List<ActionClass> GetSubweaponCards(CardDatabase cardDatabase);
}

public class PlayableEnemyWeapon : ISubWeaponType
{
    public static readonly PlayableEnemyWeapon beetleWeapons = new PlayableEnemyWeapon(name: "Beetle Cards", getSubWeaponCards: (db => db.enemyCards.FindAll(card => card is IPlayableBeetleCard).ToList()));
    public static readonly PlayableEnemyWeapon frogWeapons = new PlayableEnemyWeapon(name: "Frog Cards", getSubWeaponCards: (db => db.enemyCards.FindAll(card => card is IPlayableFrogCard).ToList()));
    public static readonly PlayableEnemyWeapon slimeWeapons = new PlayableEnemyWeapon(name: "Slime Cards", getSubWeaponCards: (db => db.enemyCards.FindAll(card => card is IPlayableSlimeCard).ToList()));
    public static readonly PlayableEnemyWeapon queenBeetleWeapons = new PlayableEnemyWeapon(name: "Queen Beetle Cards", getSubWeaponCards: (db => db.enemyCards.FindAll(card => card is IPlayableQueenCard).ToList()));
    public static readonly PlayableEnemyWeapon princessFrogWeapons = new PlayableEnemyWeapon(name: "Princess Frog Cards", getSubWeaponCards: (db => db.enemyCards.FindAll(card => card is IPlayablePrincessFrogCard).ToList()));

    public static List<ISubWeaponType> values = new()
    {
        beetleWeapons,
        frogWeapons,
        slimeWeapons,
        queenBeetleWeapons,
        princessFrogWeapons
    };

    public string Name { get; set; }
    public GetSubweaponCards GetSubWeaponCards { get; set; }

    public PlayableEnemyWeapon(string name, GetSubweaponCards getSubWeaponCards)
    {
        Name = name;
        this.GetSubWeaponCards = getSubWeaponCards;
    }
}


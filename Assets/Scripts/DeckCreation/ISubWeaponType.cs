
using static ISubWeaponType;
using System.Collections.Generic;
using System.Linq;
using static WeaponEditInformation;

public interface ISubWeaponType
{
    public string Name { get; set; }
    public GetRenderableCards GetSubWeaponCards { get; set; }
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
    public GetRenderableCards GetSubWeaponCards { get; set; }

    public PlayableEnemyWeapon(string name, GetRenderableCards getSubWeaponCards)
    {
        Name = name;
        this.GetSubWeaponCards = getSubWeaponCards;
    }
}
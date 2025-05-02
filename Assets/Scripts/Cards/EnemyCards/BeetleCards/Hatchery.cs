using Cards.EnemyCards.FrogCards;
using UnityEngine;


public class Hatchery : ActionClass, IPlayableQueenCard
{
    public GameObject[] beetlePrefabs;
    private const int HATCHERY_COST = 2;

    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;
        
        Speed = 1;
        
        description = $"Spend +{HATCHERY_COST} resonate to play this card. If this card is unstaggered, spawn 1 random beetle.";

        CostToAddToDeck = 2;
        myName = "Hatchery";
        CardType = CardType.Defense;
    }

    public override void OnQueue()
    {
        Origin.ReduceStacks(Resonate.buffName, HATCHERY_COST);
    }

    public override void OnRetrieveFromQueue()
    {
        Origin.AddStacks(Resonate.buffName, HATCHERY_COST);
    }

    public override bool IsPlayableByPlayer(out PopupType popupType)
    {
        bool isPlayable = base.IsPlayableByPlayer(out popupType);
        bool enoughStacks = Origin.GetBuffStacks(Resonate.buffName) >= HATCHERY_COST;

        popupType = enoughStacks ? popupType : PopupType.InsufficientResources;

        return isPlayable && enoughStacks;
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();

        GameObject beetle = Instantiate(beetlePrefabs[Random.Range(0, beetlePrefabs.Length)]);
        beetle.transform.SetParent(Origin.transform.parent);
        beetle.transform.transform.position = Origin.transform.position - new Vector3(0, 2, 0);
        beetle.GetComponent<EntityClass>().Team = Origin.Team;
    }

}

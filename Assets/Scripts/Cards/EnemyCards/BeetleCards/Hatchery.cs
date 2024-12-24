using UnityEngine;


public class Hatchery : ActionClass, IPlayableQueenCard
{
    public GameObject[] beetlePrefabs;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;
        
        Speed = 1;
        
        description = "Spend +2 resonate to play this card. If this card is unstaggered, spawn 1 random beetle.";

        CostToAddToDeck = 2;
        myName = "Hatchery";
        CardType = CardType.Defense;
    }


    // if origin is a queen beetle, summon a beetle on hit
    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();

        GameObject beetle = Instantiate(beetlePrefabs[Random.Range(0, beetlePrefabs.Length)]);
        beetle.transform.SetParent(Origin.transform.parent);
        beetle.transform.transform.position = Origin.transform.position - new Vector3(0, 2, 0);
        beetle.transform.localScale *= Beetle.BEETLE_SCALING;
        beetle.GetComponent<EntityClass>().Team = Origin.Team;
    }

}

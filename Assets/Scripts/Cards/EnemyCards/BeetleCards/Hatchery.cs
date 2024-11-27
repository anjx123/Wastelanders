using UnityEngine;


public class Hatchery : ActionClass
{
    public GameObject[] beetlePrefabs;
    private const float BEETLE_SCALING = 0.6f;

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 2;
        upperBound = 4;

        Speed = 1;

        description = "Spend +2 resonate to play this card. If this card is unstaggered, spawn 1 random beetle.";

        myName = "Hatchery";
        CardType = CardType.Defense;
    }


    // if origin is a queen beetle, summon a beetle on hit
    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();

        GameObject beetle = Instantiate(beetlePrefabs[Random.Range(0, beetlePrefabs.Length)]);
        beetle.transform.SetParent(Origin.transform.parent);
        beetle.transform.localScale *= BEETLE_SCALING;
        beetle.GetComponent<EntityClass>().Team = Origin.Team;
    }

}

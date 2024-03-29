using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class RightHook : FistCards
{
    [SerializeField]
    private GameObject haymakerPrefab;

    private GameObject haymaker;


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 4;

        myName = "Right Hook";
        description = "If this card is unstaggered, use Haymaker";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        if (haymaker == null) { haymaker = Instantiate(haymakerPrefab); haymaker.transform.position = new Vector3(-10, 10, 10); }
        ActionClass ac = haymaker.GetComponent<ActionClass>();
        ac.Origin = this.Origin;
        ac.Target = this.Target;
        ac.Speed = this.Speed; //Workaround for multispeeded queue 
        if (ac.Origin is PlayerClass)
        {
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(ac);
        } else
        {
            BattleQueue.BattleQueueInstance.InsertDupEnemyAction(ac);
        }

    }
}

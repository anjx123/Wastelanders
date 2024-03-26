using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LeftHook : FistCards
{
    [SerializeField]
    private GameObject rightHookPrefab;

    private GameObject righthook;
    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 2;
        upperBound = 3;
        Speed = 5;

        myName = "Left Hook";
        description = "If this card lands, use Right Hook";
        CardType = CardType.MeleeAttack;
        Renderer renderer = GetComponent<Renderer>();
        ogMaterial = renderer.material; // og sprite of card
        OriginalPosition = transform.position;
        base.Initialize();
    }

    public override void OnHit()
    {
        base.OnHit();
    }


    public override void CardIsUnstaggered()
    {
        base.CardIsUnstaggered();
        Origin.AttackAnimation("IsMelee");
        if (!righthook) { righthook = Instantiate(rightHookPrefab); righthook.transform.position = new Vector3(-10, 10, 10); }
        ActionClass ac = righthook.GetComponent<ActionClass>();
        ac.Origin = this.Origin;
        ac.Target = this.Target;
        BattleQueue.BattleQueueInstance.InsertDupPlayerAction(ac);
    }
}

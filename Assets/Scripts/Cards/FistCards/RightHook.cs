using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class RightHook : FistCards
{
    [SerializeField]
    private GameObject leftHookPrefab;

    private GameObject leftHook;

    public override void ExecuteActionEffect()
    {

    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 4;
        Speed = 4;

        myName = "Right Hook";
        description = "Hook to the Right";
       
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
        if (!leftHook) { leftHook = Instantiate(leftHookPrefab); leftHook.transform.position = new Vector3(-10, 10, 10); }
        ActionClass ac = leftHook.GetComponent<ActionClass>();
        ac.Origin = this.Origin;
        ac.Target = this.Target;
        BattleQueue.BattleQueueInstance.InsertDupPlayerAction(ac);


    }
}

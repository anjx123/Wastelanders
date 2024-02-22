
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

// Minor note: there is inconsisteny in the script naming: Hip Fire != HipFire; I added the space for the duplicates on purpose 
public class HipFire : PistolCards
{
    // @Author Muhammad 
    // NOTE: mentioned elsewhere but there's a lot of redundancy here; should have had foresight to implement a mediating class 
    protected bool original = true;

    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }


    // @Author Muhammad
    public override void CardIsUnstaggered()
    {
        if (original)
        {
            PlayerClass origin = (PlayerClass)Origin;
            List<GameObject> dupActions = origin.GetDuplicates();
            HipFireDuplicate a = null;
            // can be simplified by just getting the HipFireDuplicate Component...
            /*            foreach (GameObject o in dupActions)
                        {
                            Debug.Log(o.GetComponent<ActionClass>().GetName());
                            Debug.Log(o);

                            if (o.GetComponent<ActionClass>().GetName() == myName) // the duplicate action's name is still this things name now ain't it
                            {
                                a = Instantiate(o).GetComponent<HipFireDuplicate>(); // instantiate the prefab and get its component (which is a GameObject)
                            }
                        }*/
            a = Instantiate(dupActions[0]).GetComponent<HipFireDuplicate>(); // The object itself has a component of itself
            if (a == null)
            {
                throw new System.Exception("The card Duplicate has either not been defined or is not present inside the duplicates field.");
            }
            a.Origin = origin;
            a.Target = this.Target; // attacking twice against the same target. !!! COULD THROW AN EXCEPTION POTENTIALLY IF THE PLAYER IS DEAD UNLESS PROPER REMOVEINSTANCES ISN'T CALLED
            a.original = false;
            BattleQueue.BattleQueueInstance.InsertDupPlayerAction(a);
        }
    }


    // Start is called before the first frame update
    public override void Initialize()
    {
        lowerBound = 1;
        upperBound = 5;
        Speed = 1;
        Damage = 3;
        description = "Make This Attack Once, If Unstaggered, Make It Again";
        CardType = CardType.RangedAttack;
        myName = "HipFire";

        base.Initialize();
    }


    public override void ApplyEffect()
    {
        DupInit();
        
        Origin.ApplyAllBuffsToCard(ref duplicateCard);
    }

}

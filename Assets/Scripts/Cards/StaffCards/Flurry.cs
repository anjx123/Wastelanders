using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

// REQUIRES TESTING TODO (Haven't done this yet owing to the dearth of staff user presumably Ives.
public class Flurry : StaffCards
{
    // @Author Muhammad 
    protected bool original = true;

    public override void ExecuteActionEffect()
    {
        Debug.Log("Executing Effect");
    }

    // Start is called before the first frame update
    public override void Initialize()
    {
        
        CardType = CardType.MeleeAttack;
        myName = "Flurry";
        description = "Make This Attack Once, Then Make It Again";
        lowerBound = 2;
        upperBound = 4;
        Speed = 3;

        base.Initialize();
    }



    // @Author Muhammad
    // Note: Cards like these with duplicating functionalities ought to have a common superclass since this is a lot of redudundant repetition TODO
    public override void CardIsUnstaggered()
    {
        if (original)
        {
            PlayerClass origin = (PlayerClass)Origin;
            List<GameObject> dupActions = origin.GetDuplicates();
            FlurryDuplicate a = null;
            // >>> the objects have awareness of their actual type so you can't expect this to possible work when the actual type is very different (the component would never be returned.
            /*            foreach(GameObject o in dupActions) 
                        {
                            if (o.GetComponent<ActionClass>().GetName() == myName) // the duplicate action's name is still this things name now ain't it
                            {
                                a = Instantiate(o).GetComponent<FlurryDuplicate>(); // instantiate the prefab and get its component (which is a GameObject)
                            }
                        }*/
            a = Instantiate(dupActions[0]).GetComponent<FlurryDuplicate>(); // TODO
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
}

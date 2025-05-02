
public class Spit : FrogAttacks, IPlayableFrogCard
{

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 1;
        upperBound = 4;
        Speed = 4;
        CostToAddToDeck = 1;

        myName = "Spit";
        description = "Gross!";
    }
}

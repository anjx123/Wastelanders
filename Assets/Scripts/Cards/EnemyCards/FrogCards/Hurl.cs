
public class Hurl : FrogAttacks, IPlayableFrogCard
{
    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        lowerBound = 3;
        upperBound = 7;
        
        Speed = 5;

        CostToAddToDeck = 2;

        myName = "Hurl";
        description = "Watch out!";
    }
}

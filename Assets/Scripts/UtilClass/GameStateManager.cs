using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static Class that keeps track of static values representing general Game states
public static class GameStateManager
{
    public static bool shouldPlayDeckSelectionTutorial = false;

    public static bool jumpIntoFrogAndSlimeFight = false;

    public static bool jumpIntoBeetleFight = false;

    public static bool jumpIntoQueenFight = false;

    //If we just finished beetle fight, we go directly into queen fight after the back button is hit
    public static bool justFinishedBeetleFight = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrogHighlightManager // later all entity highlighter
{
    private static EntityClass currentHighlightedFrog;

    static FrogHighlightManager()
    {
        currentHighlightedFrog = null; // no frog highlighted
    }

    public static void OnFrogClicked(EntityClass clickedFrog)
    {
        if (currentHighlightedFrog == null)
        {
            currentHighlightedFrog = clickedFrog;
            currentHighlightedFrog.Highlight();
        }
        else if (currentHighlightedFrog != clickedFrog)
        {
            currentHighlightedFrog.DeHighlight();
            clickedFrog.Highlight();
            currentHighlightedFrog = clickedFrog;
        }
        else
        {
            currentHighlightedFrog.Toggle();
        }
    }
}

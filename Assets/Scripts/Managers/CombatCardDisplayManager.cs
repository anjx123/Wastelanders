using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CombatCardDisplayManager : MonoBehaviour
{

    public static CombatCardDisplayManager Instance;

    public GameObject cardDisplay; // The card display object
    public bool isDisplaying = false;
    public CombatCardUI combatCardUser;

    // Awake is called before Start.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this); // this is out of circumspection; unsure it this is even needed.
        }
    }


}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DescriptionManager : MonoBehaviour
{

    public static DescriptionManager Instance;
    public GameObject descriptionTextObject; // The description display object
    public GameObject backgroundSpriteObject; // the background sprite that we deactivate if nothing is hovered

    // Awake is called before Start.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

    }

    public void DisplayText(string description)
    {
        backgroundSpriteObject.GetComponent<SpriteRenderer>().enabled = true;
        descriptionTextObject.GetComponent<TextMeshPro>().text = description;
    }

    public void RemoveDescription()
    {
        backgroundSpriteObject.GetComponent<SpriteRenderer>().enabled = false;
        descriptionTextObject.GetComponent<TextMeshPro>().text = "";
    }

    
}
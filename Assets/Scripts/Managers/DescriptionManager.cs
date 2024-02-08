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
        descriptionTextObject.GetComponent<TextMeshPro>().text = description;
    }

    public void RemoveDescription()
    {
        descriptionTextObject.GetComponent<TextMeshPro>().text = "";
    }

    
}
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


/*
 * Class that holds all the sound effects for a scene 
 *  */
[CreateAssetMenu(fileName = "New Sound Effect Database", menuName = "Sound Effect Database")]
public class SoundEffectsDatabase : ScriptableObject
{
    [SerializeField] private List<AudioClip> clipList;
    private Dictionary<string, AudioClip> clipDictionary; //

    public AudioClip GetClipByName(string effectName)
    {
        if (clipDictionary == null) InitializeDictionary();

        return clipDictionary[effectName];
    }

    public void InitializeDictionary()
    {
        clipDictionary = new();
        foreach (AudioClip clip in clipList)
        {
            clipDictionary[clip.name] = clip;
            Debug.Log("The clip name being created is called " + clip.name);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance;

    // NOTE: All of these fields are set in the editor.
    public AudioSource SFXSoundsPlayer, BackgroundMusicPlayer;
    public AudioClip BackgroundMusicPrimary;
    public AudioClip BackgroundMusicVictory; // Actually ...Secondary ; for extensibility purposes such as you WANT to be able to alternate between two Background Musics during gameplay
    [SerializeField]
    private List<SerializableTuple<SFXList, AudioClip>> sfxTuples;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            BackgroundMusicPlayer.clip = BackgroundMusicPrimary;
            BackgroundMusicPlayer.Play();
            // sfxTuples = new List<SerializableTuple<SFXList, AudioClip>>();
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    // Plays the sfx that is appropriate
    // REQUIRES: the value provided is valid. 
    public void PlaySFX(SFXList effect)
    {
        foreach (SerializableTuple<SFXList, AudioClip> entry in sfxTuples) 
        {
            if (entry.Item1 == effect)
            {
                SFXSoundsPlayer.PlayOneShot(entry.Item2);
            }            
        }
    }



    public void StopMusic()
    {
        BackgroundMusicPlayer.Stop();
    }

    public void PlayVictory()
    {
        BackgroundMusicPlayer.Stop();
        BackgroundMusicPlayer.clip = BackgroundMusicVictory;
        BackgroundMusicPlayer.Play();
    }

    // You *could* want this, I suppose.
    public void SwapMusic()
    {

    }

    // this may be extended upon; it is NOT extended upon because I could not conceive in about 10 minutes why any event would change the music (exclusing victory of course)
    public enum BackgroundMusicList
    {
        None,
    }

    // enumeration of all possible sound effects playable; this serves for documentation purposes and because I couldn't find an alternate owing to paucity. 
    // each member correponds EXACTLY to a string. so pistol => "pistol". 
    // plausible solution involves a tandem list but that just seems redundant...
    public enum SFXList
    {
        pistol,
        staff,
        wastefrog_damage_taken,
        slime_damage_taken
    }
}

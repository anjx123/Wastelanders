using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    // NOTE: All of these fields are set in the editor.
    public AudioSource SFXSoundsPlayer, BackgroundMusicPlayer;
    public AudioClip BackgroundMusicPrimary;
    public AudioClip BackgroundMusicVictory; // Actually ...Secondary ; for extensibility purposes such as you WANT to be able to alternate between two Background Musics during gameplay

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        BackgroundMusicPlayer.clip = BackgroundMusicPrimary;
        BackgroundMusicPlayer.Play(); 
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSoundsPlayer.PlayOneShot(clip);
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

    // would border on bad neuroticism; besides this has to be dynamic for each Scene; there is no point in keeping redundant references. 
    // However, if need be, you could enumerate and refactor so that objects do not know what sounds they make (which seems wrong?). 
    public enum SFXList
    {

    }
}

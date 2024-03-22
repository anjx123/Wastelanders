using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NOTE: VERY IMPORTANT: This implementation requires that Audio file names be systematically maintained. Please do NOT update a name in the assets without proper understanding.
public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    // NOTE: All of these fields are set in the editor.
    public AudioSource SFXSoundsPlayer, BackgroundMusicPlayer;
    public AudioClip BackgroundMusicPrimary;
    public AudioClip BackgroundMusicVictory; // Actually ...Secondary ; for extensibility purposes such as you WANT to be able to alternate between two Background Musics during gameplay
    [SerializeField]
    private Dictionary<string, AudioClip> sfxDictionary;
    [SerializeField]
    private List<AudioClip> sfxList; 

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            BackgroundMusicPlayer.clip = BackgroundMusicPrimary;
            BackgroundMusicPlayer.Play();
            sfxDictionary = new Dictionary<string, AudioClip>();
            foreach (AudioClip entry in sfxList)
            {
                sfxDictionary[entry.name] = entry;
            }
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    // Plays the sfx that is appropriate
    // REQUIRES: the value provided is valid. 
    public void PlaySFX(string effect)
    {
        SFXSoundsPlayer.PlayOneShot(sfxDictionary[effect]);
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
        wastefrog_damage_taken
    }
}

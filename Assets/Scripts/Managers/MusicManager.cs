using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance;

    // NOTE: All of these fields are set in the editor.
    public AudioSource SFXSoundsPlayer, BackgroundMusicPlayer, BackgroundMusicIntroPlayer;

    [SerializeField]
    private List<SerializableTuple<SFXList, AudioClip>> sfxTuples = new();
#nullable enable
    public AudioClip? backgroundMusicPrimary;
    public AudioClip? backgroundMusicIntro;

    public AudioClip? combatMusicPrimary;
    public AudioClip? combatMusicIntro;

    public AudioClip? backgroundMusicDeath;

    public virtual void Awake()
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

    private void Start()
    {
        StartCoroutine(PlayStartAudio());
    }


    private void OnEnable()
    {
        CombatManager.OnGameStateChanged += CombatStartHandler;
    }

    private void OnDisable()
    {
        CombatManager.OnGameStateChanged -= CombatStartHandler;
    }

    void CombatStartHandler(GameState gameState)
    {
        if (gameState == GameState.SELECTION)
        {
            StartCoroutine(StartCombatMusic());
            
        }
    }

    protected IEnumerator StartCombatMusic()
    {
        CombatManager.OnGameStateChanged -= CombatStartHandler;
        yield return StartCoroutine(FadeAudioRoutine(BackgroundMusicPlayer, true, 1f));

        if (combatMusicIntro != null)
        {
            BackgroundMusicPlayer.clip = combatMusicIntro;
            BackgroundMusicPlayer.Play();
            BackgroundMusicPlayer.loop = false;
            yield return new WaitUntil(() => !BackgroundMusicPlayer.isPlaying);
        }
        BackgroundMusicPlayer.clip = combatMusicPrimary;
        BackgroundMusicPlayer.Play();
        BackgroundMusicPlayer.loop = true;
    }



    protected IEnumerator FadeAudioRoutine(AudioSource audioSource, bool isFadingOut, float fadeTime)
    {
        float startVolume = audioSource.volume;
        float endVolume = isFadingOut ? 0 : 1;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, time / fadeTime);

            yield return null;
        }

        if (isFadingOut)
        {
            audioSource.Stop();
            audioSource.volume = startVolume;
        } else
        {
            audioSource.volume = endVolume;
        }
    }

    public void FadeOutCurrentBackgroundTrack(float duration)
    {
        StartCoroutine(FadeAudioRoutine(BackgroundMusicPlayer, true, duration));
    }

    protected IEnumerator PlayStartAudio()
    {
        BackgroundMusicPlayer.clip = backgroundMusicIntro;
        BackgroundMusicPlayer.Play();

        yield return new WaitUntil(() => !BackgroundMusicPlayer.isPlaying);

        BackgroundMusicPlayer.clip = backgroundMusicPrimary;
        BackgroundMusicPlayer.Play();
        BackgroundMusicPlayer.loop = true;
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

    public void PlayDeath()
    {
        FadeOutCurrentBackgroundTrack(3);
        BackgroundMusicPlayer.clip = backgroundMusicDeath;
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

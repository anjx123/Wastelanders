using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

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

    public void PlaySFX(SFXList effect)
    {
        foreach (SerializableTuple<SFXList, AudioClip> entry in sfxTuples) 
        {
            if (entry.Item1 == effect)
            {
                RandomizePitch();
                SFXSoundsPlayer.PlayOneShot(entry.Item2);
            }            
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        RandomizePitch();
        SFXSoundsPlayer.PlayOneShot(audioClip);
    }    

    private void RandomizePitch()
    {
        SFXSoundsPlayer.pitch = Random.Range(0.90f, 1.1f);
    }

    public void StopMusic()
    {
        BackgroundMusicPlayer.Stop();
    }

    public void PlayDeath()
    {
        StartCoroutine(PlayDeathMusic());
    }

    IEnumerator PlayDeathMusic()
    {
        yield return StartCoroutine(FadeAudioRoutine(BackgroundMusicPlayer, true, 2f));
        BackgroundMusicPlayer.clip = backgroundMusicDeath;
        BackgroundMusicPlayer.Play();
    }

    public enum BackgroundMusicList
    {
        None,
    }


    public enum SFXList
    {
        PISTOL,
        STAFF,
        AXE,
        FIST,
        CLASH_TIE
    }
}

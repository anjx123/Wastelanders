using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : PersistentSingleton<AudioManager>
{
    public new static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if (instance == null)
                {
                    if (SceneInitializer.Instance == null)
                    {
                        Debug.LogError("AudioManager.Instance could not be created because SceneInitializer.Instance is null. " +
                                       "Ensure a SceneInitializer exists in your first scene.");
                        return null;
                    }
                    
                    instance = Instantiate(SceneInitializer.Instance.GetPrefab<AudioManager>());
                }
            }

            return instance;
        }
    }

    [SerializeField] private AudioSource SFXSoundsPlayer, BackgroundMusicPlayer, BackgroundMusicIntroPlayer;
    [SerializeField] private SoundEffectsDatabase soundEffectsDatabase;
    [SerializeField] private AudioDatabase sceneAudioDatabase;
#nullable enable
    private SceneAudio sceneAudio = null!;
    public AudioClip? backgroundMusicPrimary;
    public AudioClip? backgroundMusicIntro;
    public AudioClip? combatMusicPrimary;
    public AudioClip? combatMusicIntro;
    public AudioClip? backgroundMusicDeath;

    private void OnEnable()
    {
        CombatManager.OnGameStateChanged += CombatStartHandler;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        CombatManager.OnGameStateChanged -= CombatStartHandler;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneAudio incomingAudio = SceneData.FromSceneName(scene.name).GetAudio(sceneAudioDatabase);
        
        // This check allows us to have certain audio traacks persist across scenes 
        // E.x. MainMenu -> Level Select while also allowing audio to play on cold start
        if (sceneAudio != incomingAudio)
        {
            sceneAudio = incomingAudio;
            StartCoroutine(PlayStartAudio());
        }
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

        if (sceneAudio.combatMusicIntro != null)
        {
            BackgroundMusicPlayer.clip = sceneAudio.combatMusicIntro;
            BackgroundMusicPlayer.Play();
            BackgroundMusicPlayer.loop = false;
            yield return new WaitUntil(() => !BackgroundMusicPlayer.isPlaying);
        }
        BackgroundMusicPlayer.clip = sceneAudio.combatMusicPrimary;
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
        BackgroundMusicPlayer.clip = sceneAudio.backgroundMusicIntro;
        BackgroundMusicPlayer.Play();

        yield return new WaitUntil(() => !BackgroundMusicPlayer.isPlaying);

        BackgroundMusicPlayer.clip = sceneAudio.backgroundMusicPrimary;
        BackgroundMusicPlayer.Play();
        BackgroundMusicPlayer.loop = true;
    }

    public void PlaySFX(string effect)
    {
        RandomizePitch();
        SFXSoundsPlayer.PlayOneShot(soundEffectsDatabase.GetClipByName(effect));
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
        BackgroundMusicPlayer.clip = sceneAudio.backgroundMusicDeath;
        BackgroundMusicPlayer.Play();
    }

    public void ToggleSFX()
    {
        SFXSoundsPlayer.mute = !SFXSoundsPlayer.mute;
    }

    public void ToggleMusic()
    {
        BackgroundMusicIntroPlayer.mute = !BackgroundMusicIntroPlayer.mute;
        BackgroundMusicPlayer.mute = !BackgroundMusicPlayer.mute;
    }

    public void SFXVolume(float volume)
    {
        SFXSoundsPlayer.volume = volume;
    }

    public void MusicVolume(float volume)
    {
        BackgroundMusicIntroPlayer.volume = volume;
        BackgroundMusicPlayer.volume = volume;
    }
}

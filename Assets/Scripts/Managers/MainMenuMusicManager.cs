using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusicManager : MusicManager
{

    public static MusicManager MainMenuMusicInstance;

    public override void Awake()
    {
        if (MainMenuMusicInstance == null)
        {
            DontDestroyOnLoad(transform.gameObject);
            MainMenuMusicInstance = this;
        }
        else if (MainMenuMusicInstance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        StartCoroutine(PlayStartAudio());
    }


    IEnumerator PlayStartAudio()
    {
        BackgroundMusicPlayer.clip = backgroundMusicIntro;
        BackgroundMusicPlayer.Play();

        yield return new WaitUntil(() => !BackgroundMusicPlayer.isPlaying);

        BackgroundMusicPlayer.clip = backgroundMusicPrimary;
        BackgroundMusicPlayer.Play();
        BackgroundMusicPlayer.loop = true;
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameStateManager.SELECTION_SCREEN_NAME && scene.name != GameStateManager.MAIN_MENU_NAME && scene.name != GameStateManager.LEVEL_SELECT_NAME)
        {
            StartCoroutine(RunOnScreenLoaded(2f));
        }
    }

    IEnumerator RunOnScreenLoaded(float duration)
    {
        yield return StartCoroutine(FadeAudioRoutine(BackgroundMusicPlayer, true, duration));
        Destroy(this.gameObject);
    }
}

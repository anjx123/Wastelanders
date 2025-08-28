using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Director
{
    public class SplashScreenDirector : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private FadeScreenHandler fadeScreenHandler;

        private void Start()
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            StartCoroutine(StartSequence());
        }

        private IEnumerator StartSequence()
        {
            videoPlayer.Play();
            yield return null;
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            StartCoroutine(EndSequence());
        }

        private IEnumerator EndSequence()
        {
            yield return new WaitForSeconds(1f);
            yield return fadeScreenHandler.FadeInDarkScreen(1f);
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
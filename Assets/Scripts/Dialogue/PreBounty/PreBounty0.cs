using System.Collections;
using UnityEngine;

namespace Dialogue.PreBounty {
    public class PreBounty0 : MonoBehaviour {
        [SerializeField] private DialogueWrapper ailinDialogue;
        private void Start() {
            StartCoroutine(PlayScene());
        }

        private IEnumerator PlayScene() {
            UIFadeScreenManager.Instance.SetDarkScreen();
            yield return UIFadeScreenManager.Instance.FadeInLightScreen(2f);
            
            yield return DialogueManager.Instance.StartDialogue(ailinDialogue.Dialogue);
            
        }
    }
}
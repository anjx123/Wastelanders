using System;
using System.Collections;
using DialogueScripts;
using UnityEngine;

namespace Dialogue.PreBounty
{
    public class PreBounty2 : MonoBehaviour
    {
        [SerializeField] private DialogueEntryInUnityEditor[] dialogue;

        private IEnumerator Start()
        {
            UIFadeScreenManager.Instance.SetDarkScreen();
            yield return UIFadeScreenManager.Instance.FadeInLightScreen(2f);
            yield return new WaitForSeconds(1f);
            yield return DialogueBoxV2.Instance.Play(dialogue.Into());
            yield return UIFadeScreenManager.Instance.FadeInDarkScreen(2f);
        }

        private void OnEnable()
        {
            DialogueBoxV2.Instance.OnDialogueEntryWithSetEventId += HandleDialogueEvent;
        }

        private void OnDisable()
        {
            DialogueBoxV2.Instance.OnDialogueEntryWithSetEventId -= HandleDialogueEvent;
        }

        private static void HandleDialogueEvent(string eventId)
        {
            foreach (var id in eventId.Split(';'))
            {
                switch (id)
                {
                    case "auto":
                        DialogueBoxV2.Instance.SetAutoAdvance();
                        break;
                    case "c-ne":
                    case "c-po":
                    case "c-pr":
                    case "c-sm":
                    case "c-ta":
                        Debug.Log("emote cam: " + id);
                        break;
                    case "in-c":
                        Debug.Log("intro cam");
                        break;
                    case "in-j":
                        Debug.Log("intro jackie");
                        break;
                    case "j-mo":
                    case "j-sc":
                    case "j-se":
                    case "j-sm":
                    case "j-so":
                        Debug.Log("emote jackie: " + id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
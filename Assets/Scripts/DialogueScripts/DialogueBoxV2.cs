using System;
using System.Collections;
using TMPro;
using UI_Toolkit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueScripts
{
    public class DialogueBoxV2 : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtView;
        [SerializeField] private TextMeshProUGUI whoView;
        [SerializeField] private Image imgView;
        [SerializeField] private RectTransform boxLayout;

        [SerializeField] private GameObject txt;
        [SerializeField] private GameObject who;
        [SerializeField] private GameObject img;

        [SerializeField] private int typewriterRate = 50;

#nullable enable
        public static DialogueBoxV2 Instance { get; private set; }

        private AutoAdvanceAfter? autoAdvanceAfter;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            this.AddComponent<SetAutoHandler>().Subscribe(SetAutoAdvance);
            this.AddComponent<VerticalLayoutHandler>().Subscribe(SetVerticalLayout);
            
            gameObject.SetActive(false);
        }

        public IEnumerator Play(DialogueEntry[] entries)
        {
            gameObject.SetActive(true);

            foreach (var entry in entries)
            {
                WithEntry(entry);

                var elapsed = 0f;
                txtView.maxVisibleCharacters = 0;

                while (txtView.maxVisibleCharacters <= txtView.text.Length)
                {
                    if (HasInput())
                    {
                        txtView.maxVisibleCharacters = txtView.text.Length;
                        yield return null;
                        break;
                    }

                    txtView.maxVisibleCharacters = Mathf.FloorToInt(elapsed * typewriterRate);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                yield return new WaitUntil(() => HasInput() || autoAdvanceAfter is not null);

                if (autoAdvanceAfter is not null)
                {
                    yield return new WaitForSeconds(autoAdvanceAfter.Time);
                    autoAdvanceAfter = null;
                }

                if (string.IsNullOrEmpty(entry.sfxName) && !Input.GetKey(KeyCode.RightArrow))
                {
                    const string DEFAULT_SFX_NAME = "Page Flip";
                    AudioManager.Instance.PlaySFX(DEFAULT_SFX_NAME);
                }

                yield return null;
            }

            gameObject.SetActive(false);
        }

        private void SetAutoAdvance(AutoAdvanceAfter e)
        {
            autoAdvanceAfter = e;
        }

        private void SetVerticalLayout(VerticalLayoutChange e)
        {
            const float BOX_Y = 318.9375f;
            boxLayout.anchoredPosition = BOX_Y * e.Layout switch
            {
                Layout.LOWER => Vector2.down,
                Layout.UPPER => Vector2.up,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void WithEntry(DialogueEntry entry)
        {
            if (!string.IsNullOrEmpty(entry.content))
            {
                txtView.text = entry.content;
                txt.SetActive(true);
            }
            else
            {
                txt.SetActive(false);
            }

            if (!string.IsNullOrEmpty(entry.speaker))
            {
                whoView.text = entry.speaker.ToUpper();
                who.SetActive(true);
            }
            else
            {
                who.SetActive(false);
            }

            if (!string.IsNullOrEmpty(entry.sfxName))
            {
                AudioManager.Instance.PlaySFX(entry.sfxName);
            }

            if (entry.picture)
            {
                imgView.sprite = entry.picture;
                img.SetActive(true);
            }
            else
            {
                img.SetActive(false);
            }

            entry.events.ForEach(it => it.Execute());

            DialogueManager.Instance.AddDialogueEntryToHistory(entry);
        }

        private static bool HasInput() => !PauseMenuV2.IsPaused && (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space));
        private class SetAutoHandler: EventHandler<AutoAdvanceAfter> { }
        private class VerticalLayoutHandler : EventHandler<VerticalLayoutChange> { }
    }
}
using System;
using System.Collections;
using TMPro;
using UI_Toolkit;
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

        public static DialogueBoxV2 Instance { get; private set; }
        public event Action<string> OnDialogueEntryWithSetEventId;

        private bool isAutoAdvance;

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

            gameObject.SetActive(false);
        }

        public void SetAutoAdvance()
        {
            isAutoAdvance = true;
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

            if (!string.IsNullOrEmpty(entry.eventId))
            {
                OnDialogueEntryWithSetEventId?.Invoke(entry.eventId);
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

            const float BOX_Y = 318.9375f;
            boxLayout.anchoredPosition = BOX_Y * entry.verticalLayout switch
            {
                Layout.LOWER => Vector2.down,
                Layout.UPPER => Vector2.up,
                _ => throw new ArgumentOutOfRangeException()
            };

            DialogueManager.Instance.AddDialogueEntryToHistory(entry);
        }

        public IEnumerator Play(DialogueEntry[] entries)
        {
            gameObject.SetActive(true);

            foreach (var entry in entries)
            {
                isAutoAdvance = false;

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

                if (isAutoAdvance)
                {
                    continue;
                }

                while (true)
                {
                    if (HasInput())
                    {
                        if (string.IsNullOrEmpty(entry.sfxName) && !Input.GetKey(KeyCode.RightArrow))
                        {
                            const string DEFAULT_SFX_NAME = "Page Flip";
                            AudioManager.Instance.PlaySFX(DEFAULT_SFX_NAME);
                        }

                        yield return null;
                        break;
                    }

                    yield return null;
                }
            }

            gameObject.SetActive(false);
        }

        private static bool HasInput() => !PauseMenuV2.IsPaused && (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space));
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Toolkit
{
    public class PauseMenuV2 : MonoBehaviour
    {
        public UIDocument rootDocument;
        public Canvas inputBlockCanvas;

        private VisualElement rootElem;
        private VisualElement dialogue;
        private VisualElement glossary;
        private VisualElement pauseMenuPanel;

        private State state;

        // Retained legacy cruft for compat.
        public static bool IsPaused;
        public static event Action DidPause;

        public void Awake()
        {
            rootElem = rootDocument?.rootVisualElement ?? throw new Exception($"{nameof(rootDocument)} unset");
            dialogue = rootElem.Q<VisualElement>("dialogue");
            glossary = rootElem.Q<VisualElement>("glossary");
            pauseMenuPanel = rootElem.Q<VisualElement>("pause-menu-panel");

            RegisterCallbacks();
            LoadInitialValues();
            SetState(State.Unpaused);
        }

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Tab)) return;
            if (state != State.Unpaused) DoStart();
            else DoPause();
        }

        private void DoPause()
        {
            SetState(State.PauseMenuPanel);
            Time.timeScale = 0;
            DidPause?.Invoke();
        }

        private void DoStart()
        {
            SetState(State.Unpaused);
            Time.timeScale = 1;
        }

        private void SetState(State to)
        {
            IsPaused = inputBlockCanvas.enabled = to != State.Unpaused;
            state = to;

            rootElem.style.display = to != State.Unpaused ? DisplayStyle.Flex : DisplayStyle.None;
            dialogue.style.display = to == State.Dialogue ? DisplayStyle.Flex : DisplayStyle.None;
            glossary.style.display = to == State.Glossary ? DisplayStyle.Flex : DisplayStyle.None;
            pauseMenuPanel.style.display = to == State.PauseMenuPanel ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnRsmClicked()
        {
            DoStart();
        }

        private void OnRstClicked()
        {
            DoStart();
            GameStateManager.Instance.Restart();
        }

        private void OnDckClicked()
        {
            DoStart();
            GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.SelectionScreen>().SceneName);
        }

        private void OnLvlClicked()
        {
            DoStart();
            GameStateManager.Instance.LoadScene(SceneData.Get<SceneData.LevelSelect>().SceneName);
        }

        private void OnGlsClicked()
        {
            SetState(State.Glossary);
        }

        private void OnDlgClicked()
        {
            SetState(State.Dialogue);
            var scroll = dialogue.Q<ScrollView>("scroll-dlg");
            StartCoroutine(DoScrollToBottomWithDelay(scroll));
            scroll.Clear();
            foreach (var it in CreateLabels()) scroll.Add(it);
        }

        private void OnClsClicked()
        {
            SetState(State.PauseMenuPanel);
        }

        private static void OnMusChanged(float value)
        {
            AudioManager.Instance.MusicVolume(value);
        }

        private static void OnSfxChanged(float value)
        {
            AudioManager.Instance.SFXVolume(value);
        }

        private static void OnMusChecked()
        {
            AudioManager.Instance.ToggleMusic();
        }

        private static void OnSfxChecked()
        {
            AudioManager.Instance.ToggleSFX();
        }

        private static void OnVfxChecked(bool value)
        {
            ScreenShakeHandler.IsScreenShakeEnabled = value;
        }

        private void RegisterCallbacks()
        {
            pauseMenuPanel.Q<Button>("button-rsm").clicked += OnRsmClicked;
            pauseMenuPanel.Q<Button>("button-rst").clicked += OnRstClicked;
            pauseMenuPanel.Q<Button>("button-dck").clicked += OnDckClicked;
            pauseMenuPanel.Q<Button>("button-lvl").clicked += OnLvlClicked;
            pauseMenuPanel.Q<Button>("button-gls").clicked += OnGlsClicked;
            pauseMenuPanel.Q<Button>("button-dlg").clicked += OnDlgClicked;

            dialogue.Q<Button>("button-cls").clicked += OnClsClicked;
            glossary.Q<Button>("button-cls").clicked += OnClsClicked;

            pauseMenuPanel.Q<Slider>("slider-mus").RegisterValueChangedCallback(e => OnMusChanged(e.newValue));
            pauseMenuPanel.Q<Slider>("slider-sfx").RegisterValueChangedCallback(e => OnSfxChanged(e.newValue));
            pauseMenuPanel.Q<Toggle>("toggle-mus").RegisterValueChangedCallback(_ => OnMusChecked());
            pauseMenuPanel.Q<Toggle>("toggle-sfx").RegisterValueChangedCallback(_ => OnSfxChecked());
            pauseMenuPanel.Q<Toggle>("toggle-vfx").RegisterValueChangedCallback(e => OnVfxChecked(e.newValue));
        }

        private void LoadInitialValues()
        {
            // TODO: Load these values from saved settings and sync with audio manager.
            pauseMenuPanel.Q<Slider>("slider-mus").value = 0.2f;
            pauseMenuPanel.Q<Slider>("slider-sfx").value = 0.2f;
            pauseMenuPanel.Q<Toggle>("toggle-vfx").value = ScreenShakeHandler.IsScreenShakeEnabled;
        }

        private enum State
        {
            Unpaused,
            Dialogue,
            Glossary,
            PauseMenuPanel
        }

        private static IEnumerator DoScrollToBottomWithDelay(ScrollView scroll)
        {
            yield return null;
            scroll.scrollOffset = scroll.contentContainer.layout.max - scroll.contentViewport.layout.size;
        }

        // Hideous.
        private static IEnumerable<VisualElement> CreateLabels()
        {
            var history = DialogueManager.Instance.GetHistory();
            if (history.Count == 0) yield break;
            for (var i = 0; i < history.Count; i++)
            {
                if (history[i].SpeakerName != "" && (i == 0 || history[i].SpeakerName != history[i - 1].SpeakerName))
                {
                    var label1 = new Label($"<b>{history[i].SpeakerName.Trim()}</b>")
                        { style = { marginTop = i == 0 ? 0 : 32 } };
                    label1.AddToClassList("dynamic-dialogue-name");
                    yield return label1;
                }

                var label2 = new Label(history[i].BodyText.Trim()) { style = { marginTop = i == 0 ? 0 : 16 } };
                label2.AddToClassList("dynamic-dialogue-text");
                yield return label2;
            }
        }
    }
}
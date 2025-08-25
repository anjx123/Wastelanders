using System;
using System.Collections;
using System.Collections.Generic;
using UI_Toolkit.UI_Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Toolkit
{
    public class HUDV2 : MonoBehaviour
    {
        public UIDocument rootDocument;

        private VisualElement rootElem;
        private VisualElement cardInfo;

        public void Awake()
        {
            rootElem = rootDocument?.rootVisualElement ?? throw new Exception($"{nameof(rootDocument)} unset");
            cardInfo = rootElem.Q<VisualElement>("layout-card-ui");

            RegisterCallbacks();
            cardInfo.style.display = DisplayStyle.None;
        }

        public void OnEnable()
        {
            CombatManager.OnGameStateChanging += OnGameStateChanging;
            DisplayableClass.OnShowCard += OnShowCardInfo;
            DisplayableClass.OnHideCard += OnHideCardInfo;

            ActionClass.CardHighlightedEvent += OnShowCardInfo;
            ActionClass.CardUnhighlightedEvent += OnHideCardInfo;
        }

        public void OnDisable()
        {
            CombatManager.OnGameStateChanging -= OnGameStateChanging;
            DisplayableClass.OnShowCard -= OnShowCardInfo;
            DisplayableClass.OnHideCard -= OnHideCardInfo;

            ActionClass.CardHighlightedEvent -= OnShowCardInfo;
            ActionClass.CardUnhighlightedEvent -= OnHideCardInfo;
        }

        private void OnGameStateChanging(GameState to)
        {
            rootElem.style.display = to switch
            {
                GameState.FIGHTING or GameState.OUT_OF_COMBAT => DisplayStyle.None,
                GameState.SELECTION => DisplayStyle.Flex,
                _ => rootElem.style.display
            };
        }

        private void OnShowCardInfo(ActionClass ac)
        {
            var card = cardInfo.Q<TemplateContainer>("CardV2").Q<CardV2>();
            card.WithAttrsFromActionClass(ac);

            var text = cardInfo.Q<Label>("label-card-txt");
            text.text = ac.GenerateCardDescription();
            text.ClearClassList();
            text.AddToClassList(ac.IsPlayedByPlayer() ? "dynamic-bg-player" : "dynamic-bg-enemy");

            cardInfo.style.display = DisplayStyle.Flex;
        }

        private void OnHideCardInfo(ActionClass ac)
        {
            cardInfo.style.display = DisplayStyle.None;
        }

        private static void OnConfirmClicked()
        {
            BattleQueue.BattleQueueInstance.BeginDequeue();
        }

        private void RegisterCallbacks()
        {
            rootElem.Q<Button>("button-confirm").clicked += OnConfirmClicked;
        }
    }
}
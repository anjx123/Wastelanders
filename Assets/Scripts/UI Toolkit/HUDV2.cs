using System;
using System.Collections.Generic;
using UI_Toolkit.UI_Elements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Toolkit
{
    public class HUDV2 : MonoBehaviour
    {
        public VisualTreeAsset cardTemplate;
        public UIDocument rootDocument;

        private VisualElement rootElem;
        private VisualElement handElem;
        private VisualElement infoElem;

        public void Awake()
        {
            rootElem = rootDocument?.rootVisualElement ?? throw new Exception($"{nameof(rootDocument)} unset");
            handElem = rootElem.Q<VisualElement>("layout-hand");
            infoElem = rootElem.Q<VisualElement>("layout-info");

            RegisterCallbacks();
            handElem.Clear();
            infoElem.style.display = DisplayStyle.None;
        }

        public void OnEnable()
        {
            CombatManager.OnGameStateChanging += OnGameStateChanging;
            DisplayableClass.OnShowCard += OnShowCardInfo;
            DisplayableClass.OnHideCard += OnHideCardInfo;
            HighlightManager.OnUpdateHand += OnUpdateHand;

            ActionClass.CardHighlightedEvent += OnShowCardInfo;
            ActionClass.CardUnhighlightedEvent += OnHideCardInfo;
        }

        public void OnDisable()
        {
            CombatManager.OnGameStateChanging -= OnGameStateChanging;
            DisplayableClass.OnShowCard -= OnShowCardInfo;
            DisplayableClass.OnHideCard -= OnHideCardInfo;
            HighlightManager.OnUpdateHand -= OnUpdateHand;

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
            var card = infoElem.Q<TemplateContainer>("CardV2").Q<CardV2>();
            card.WithAttrsFromActionClass(ac);

            var desc = infoElem.Q<Label>("label-desc");
            desc.text = ac.GenerateCardDescription();
            desc.ClearClassList();
            desc.AddToClassList(ac.IsPlayedByPlayer() ? "dynamic-bg-player" : "dynamic-bg-enemy");

            infoElem.style.display = DisplayStyle.Flex;
        }

        private void OnHideCardInfo(ActionClass ac)
        {
            infoElem.style.display = DisplayStyle.None;
        }

        private static void OnConfirmClicked()
        {
            BattleQueue.BattleQueueInstance.BeginDequeue();
        }

        private void RegisterCallbacks()
        {
            rootElem.Q<Button>("button-confirm").clicked += OnConfirmClicked;
        }

        private void OnUpdateHand(List<ActionClass> hand)
        {
            handElem.Clear();
            foreach (var ac in hand)
            {
                var cardLayout = cardTemplate.Instantiate();
                cardLayout.ClearClassList();
                cardLayout.AddToClassList("dynamic-card");

                var card = cardLayout.Q<CardV2>();
                card.WithAttrsFromActionClass(ac);

                card.RegisterCallback<MouseDownEvent>(_ => ac.OnMouseDown());
                card.RegisterCallback<MouseEnterEvent>(_ => ac.OnMouseEnter());
                card.RegisterCallback<MouseLeaveEvent>(_ => ac.OnMouseExit());

                handElem.Add(cardLayout);
            }
        }
    }
}
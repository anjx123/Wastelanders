using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Toolkit
{
    public class HUDV2 : MonoBehaviour
    {
        public UIDocument rootDocument;
        private VisualElement rootElem;

        public void Awake()
        {
            rootElem = rootDocument?.rootVisualElement ?? throw new Exception($"{nameof(rootDocument)} unset");
            RegisterCallbacks();
        }

        public void OnEnable()
        {
            CombatManager.OnGameStateChanging += OnGameStateChanging;
        }

        public void OnDisable()
        {
            CombatManager.OnGameStateChanging -= OnGameStateChanging;
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

        private static void OnCnfClicked()
        {
            BattleQueue.BattleQueueInstance.BeginDequeue();
        }

        private void RegisterCallbacks()
        {
            rootElem.Q<Button>("button-cnf").clicked += OnCnfClicked;
        }
    }
}
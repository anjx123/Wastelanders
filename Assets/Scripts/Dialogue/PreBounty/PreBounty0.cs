using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.PreBounty {
    public class PreBounty0 : MonoBehaviour {
        [SerializeField] private DialogueWrapper ailinDialogue;
        [SerializeField] private Camera camera;
        [SerializeField] private List<Transform> keyframes;
        [SerializeField] private List<float> zoomValues; // ⬅ Add this
        [SerializeField] private float panDuration = 1.5f;
        
        private int keyframeIndex;
        private Coroutine panCoroutine;
        
        private void Start() {
            keyframeIndex = 0;
            StartCoroutine(PlayScene());
        }

        private void OnEnable() {
            DialogueBox.DialogueBoxEvent += HandleDialogueEvent;
        }

        private void OnDisable() {
            DialogueBox.DialogueBoxEvent -= HandleDialogueEvent;
        }

        private IEnumerator PlayScene() {
            UIFadeScreenManager.Instance.SetDarkScreen();
            yield return UIFadeScreenManager.Instance.FadeInLightScreen(2f);
            yield return DialogueManager.Instance.StartDialogue(ailinDialogue.Dialogue);
            yield return UIFadeScreenManager.Instance.FadeInDarkScreen(3f);
        }

        private void HandleDialogueEvent() {
            if (keyframeIndex >= keyframes.Count)
                return;

            if (panCoroutine != null)
                StopCoroutine(panCoroutine);
            
            panCoroutine = StartCoroutine(PanCameraToKeyframe(keyframes[keyframeIndex], zoomValues[keyframeIndex]));
            keyframeIndex++;
        }

        private IEnumerator PanCameraToKeyframe(Transform keyframe, float targetZoom)
        {
            Vector3 startPos = camera.transform.position;
            Quaternion startRot = camera.transform.rotation;
            float startZoom = camera.orthographicSize;

            Vector3 endPos = keyframe.position;
            Quaternion endRot = keyframe.rotation;

            float elapsed = 0f;

            while (elapsed < panDuration)
            {
                float t = elapsed / panDuration;
                t = EaseInOutQuad(t); // quadratic easing

                camera.transform.position = Vector3.Lerp(startPos, endPos, t);
                camera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                camera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Snap to final position
            camera.transform.position = endPos;
            camera.transform.rotation = endRot;
            camera.fieldOfView = targetZoom;

            panCoroutine = null;
        }

        // --- Quadratic easing helpers ---
        private float EaseInOutQuad(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }
    }
}

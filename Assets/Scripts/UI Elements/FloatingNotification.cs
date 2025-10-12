using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNotification : MonoBehaviour
{
    public float fadeInDurationS = 0.2f;
    public float fadeOutDurationS = 0.5f;
    public float floatDurationS = 1.0f;
    public float floatVelocity = 1f;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Rigidbody2D rb;

    void Awake()
    {
        rb.velocity = Vector2.up * floatVelocity;
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.alpha = 0f;
    }

    public void Initialize(Vector2 position, string text) {
        transform.position = position;
        GetComponent<TMPro.TMP_Text>().text = text;
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine() {
        yield return StartCoroutine(Fade(0f, 1f, fadeInDurationS));
        yield return new WaitForSeconds(floatDurationS);
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDurationS));
        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            var value = t / duration;
            t += Time.deltaTime;
            textMesh.alpha = Mathf.SmoothStep(from, to, value);
            yield return null;
        }
        textMesh.alpha = to;
    }
}
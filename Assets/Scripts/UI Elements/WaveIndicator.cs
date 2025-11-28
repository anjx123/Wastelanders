using TMPro;
using UnityEngine;

namespace UI_Elements
{
    public class WaveIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void Show(int cur, int max)
        {
            text.text = $"{cur}/{max}";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace UI_Elements
{
    public class DeckRemainderViewController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text;
        public void Notify(List<GameObject> pool) => text.text = $"Remaining cards: {pool.Count}";
    }
}
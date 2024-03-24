using TMPro;
using UnityEngine;

public class PointsAmount : MonoBehaviour
{
    public TMP_Text points;

    public void Start()
    {
        if (points == null)
        {
            points = GetComponent<TMP_Text>();
        }
    }
    public void TextUpdate(string message)
    {
        if (points == null)
        {
            points = GetComponent<TMP_Text>();
        }

        points.SetText(message);
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] [TextArea] private string prefix;
    [SerializeField] private int currentWave;
    [SerializeField] private int totalWaves;
    
    public void SetWave(int wave, bool setVisible = true) {
        currentWave = wave;
        text.text = prefix + currentWave + '/' + totalWaves;
        gameObject.SetActive(setVisible);
    }
}

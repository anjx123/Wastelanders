using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance { get; private set; }

    public GameObject warningPrefab;
    public GameObject canvasObject;

    public bool hover = false;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);
        StartCoroutine(createWarning("ahh!!!"));
    }

    public void WarningSwitch(PopupType popupType, GameObject obj = null)
    {
        switch (popupType)
        {
            case PopupType.SameSpeed:
                StartCoroutine(createWarning("Multiple Cards with Same Speed Selected"));
                break;
            case PopupType.EnemyKilled:
                StartCoroutine(createWarning("Enemy Killed!"));
                break;
            case PopupType.DeckReshuffled:
                StartCoroutine(createWarning("Deck Reshuffled"));
                break;
            default:
                break;
        }

    }

    public IEnumerator createWarning(string message)
    {
        GameObject instance = Instantiate(warningPrefab, canvasObject.transform);
        WarningInfo info = instance.GetComponent<WarningInfo>();
        info.setText(message);

        yield return new WaitForSeconds(8);



        Destroy(instance);
    }

    public void Hover()
    {
        hover = true;
    }

    public void UnHover() { hover = false; }


}
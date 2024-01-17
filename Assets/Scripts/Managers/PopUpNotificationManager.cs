using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


public class PopUpNotificationManager : MonoBehaviour
{
    public static PopUpNotificationManager Instance {  get; private set; }

    public GameObject warningPrefab;
    public GameObject canvasObject;

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

    public IEnumerator createWarning(string message)
    {
        GameObject instance = Instantiate(warningPrefab, canvasObject.transform);
        WarningInfo info = instance.GetComponent<WarningInfo>();
        info.setText(message);

        yield return new WaitForSeconds(20);

       // Destroy(instance);
    }

   
}

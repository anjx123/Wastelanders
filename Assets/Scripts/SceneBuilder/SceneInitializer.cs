using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public static SceneInitializer Instance { get; private set; }
    public AudioManager audioManagerPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
// SceneInitializer.cs (Upgraded)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneInitializer : MonoBehaviour
{
    public static SceneInitializer Instance { get; private set; }
    [SerializeField] private SceneInitializerPrefabs initializablePrefabs;
    public SceneInitializerPrefabs InitializablePrefabs => initializablePrefabs;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        InitializeManagersForScene();
    }

    private void InitializeManagersForScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        var requiredManagers = SceneData.FromSceneName(currentSceneName).RequiredPrefabs(initializablePrefabs);
        var managersParent = new GameObject("[Managers]");
        
        foreach (var managerPrefab in requiredManagers)
        {
            var newManagerInstance = Instantiate(managerPrefab, managersParent.transform);
            newManagerInstance.name = managerPrefab.name;
        }
    }
}

[Serializable]
public class SceneInitializerPrefabs
{
    public AudioManager audioManager;
}
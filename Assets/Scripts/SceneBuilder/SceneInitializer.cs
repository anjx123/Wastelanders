// SceneInitializer.cs (Upgraded)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#nullable enable
public class SceneInitializer : MonoBehaviour
{
    public static SceneInitializer Instance { get; private set; } = null!;
    [SerializeField] private SceneInitializerPrefabs initializablePrefabs = null!;
    public SceneInitializerPrefabs InitializablePrefabs => initializablePrefabs;

    private GameObject managersParent = null!;

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
        managersParent = new GameObject("[SceneInitialized]");
        
        foreach (var managerPrefab in requiredManagers)
        {
            InstantiatePrefab(managerPrefab);
        }
    }

    public T InstantiatePrefab<T>(T prefab) where T : MonoBehaviour
    {
        var newManagerInstance = Instantiate(prefab, managersParent.transform);
        newManagerInstance.name = prefab.name;
        return newManagerInstance;
    }
}

[Serializable]
public class SceneInitializerPrefabs
{
    public AudioManager audioManager = null!;
    public BattleIntro battleIntro = null!;
}
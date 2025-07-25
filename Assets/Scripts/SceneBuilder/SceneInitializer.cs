// SceneInitializer.cs (Upgraded)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneInitializer : MonoBehaviour
{
    public static SceneInitializer Instance { get; private set; }

    [Header("Manager Registry")]
    [Tooltip("A list of all possible manager prefabs that can be instantiated by this system.")]
    [SerializeField] private List<MonoBehaviour> managerPrefabs = new List<MonoBehaviour>();
    private Dictionary<Type, MonoBehaviour> _managerPrefabMap;

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

    public T GetPrefab<T>() where T : MonoBehaviour
    {
        _managerPrefabMap.TryGetValue(typeof(T), out var manager);
        return manager as T;
    }

    private void InitializeManagersForScene()
    {
        _managerPrefabMap = managerPrefabs.ToDictionary(p => p.GetType(), p => p);

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneData sceneData = SceneData.FromSceneName(currentSceneName);

        if (sceneData == null)
        {
            Debug.LogWarning($"No SceneData blueprint found for scene '{currentSceneName}'. No managers will be auto-instantiated.");
            return;
        }

        var requiredManagers = sceneData.RequiredManagerTypes;
        if (requiredManagers == null || !requiredManagers.Any()) return;

        var managersParent = GameObject.Find("[Managers]");
        if (managersParent == null)
        {
            managersParent = new GameObject("[Managers]");
        }

        foreach (var managerType in requiredManagers)
        {
            if (FindObjectOfType(managerType) != null)
            {
                continue;
            }

            if (_managerPrefabMap.TryGetValue(managerType, out var managerPrefab))
            {
                var newManagerInstance = Instantiate(managerPrefab, managersParent.transform);
                newManagerInstance.name = managerPrefab.name;
            }
            else
            {
                Debug.LogError($"Scene '{currentSceneName}' requires a manager of type '{managerType.Name}', " +
                               $"but it was not found in the SceneInitializer's manager registry.", this);
            }
        }
    }
}
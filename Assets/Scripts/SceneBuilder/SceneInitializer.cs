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

        // 3. Get the list of required managers from the blueprint.
        var requiredManagers = sceneData.RequiredManagerTypes;
        if (requiredManagers == null || !requiredManagers.Any()) return;

        // 4. Find or create a parent object for organization.
        var managersParent = GameObject.Find("[Managers]");
        if (managersParent == null)
        {
            managersParent = new GameObject("[Managers]");
        }

        // 5. Instantiate each required manager if it doesn't already exist.
        foreach (var managerType in requiredManagers)
        {
            // Check if a persistent instance of this manager already exists.
            if (FindObjectOfType(managerType) != null)
            {
                // Instance already exists (likely from a previous scene), so we skip it.
                continue;
            }

            // Find the corresponding prefab from our registry.
            if (_managerPrefabMap.TryGetValue(managerType, out var managerPrefab))
            {
                // Instantiate the prefab under the [Managers] parent.
                var newManagerInstance = Instantiate(managerPrefab, managersParent.transform);
                newManagerInstance.name = managerPrefab.name; // Keep the prefab's name.
            }
            else
            {
                Debug.LogError($"Scene '{currentSceneName}' requires a manager of type '{managerType.Name}', " +
                               $"but it was not found in the SceneInitializer's manager registry.", this);
            }
        }
    }
}
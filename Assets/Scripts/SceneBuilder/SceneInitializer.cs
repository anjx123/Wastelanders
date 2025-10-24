// SceneInitializer.cs (Upgraded)
using System;
using System.Collections.Generic;
using System.Linq;
using UI_Toolkit;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

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
            // Check if a persistent instance of this manager already exists.
            if (FindObjectOfType(managerPrefab.GetType()) != null)
            {
                Debug.LogWarning($"{managerPrefab.GetType()} already exists!");
                continue;
            }
            InstantiatePrefab(managerPrefab);
        }
    }

    public T InstantiatePrefab<T>(T prefab) where T : MonoBehaviour
    {
        var newManagerInstance = Instantiate(prefab, managersParent.transform);
        newManagerInstance.name = prefab.name;
        return newManagerInstance;
    }

    private CombatManager combatManager;
    private AudioManager audioManager;

    public void TestListen()
    {
        this.AddComponent<DepedencyHandler>().Subscribe(Handler);
    }
    private void Handler(GetDepedency dependency)
    {
        dependency.command.HandleLocator(InitializablePrefabs);
    }

    private void TestSend()
    {
        new ServiceLocatorInterfacer()
            .GetDependency(out combatManager)
            .GetDependency(out audioManager);
    }

    // If you want a dependency from the 
    public class ServiceLocatorInterfacer
    {
        public ServiceLocatorInterfacer GetDependency<T>(out T dependency)
        {
            dependency = GetterCommand<T>.InvokeGet();
            return this;
        }

    }
    private class DepedencyHandler : EventHandler<GetDepedency> { }
}


[Serializable]
public class SceneInitializerPrefabs: DependencyProvider
{
    public AudioManager audioManager = null!;
    public BattleIntro battleIntro = null!;
    public PauseMenuV2 pauseMenuV2 = null!;
    public HUDV2 hudV2 = null!;
    public DialogueManager dialogueManager = null!;
    public DeckSelectV2 deckSelectV2 = null!;
    public UIFadeScreenManager uiFadeScreenManager = null!;
    public CombatFadeScreenHandler combatFadeScreenManager = null!;
    public PopUpNotificationManager popupManager = null!;
    public GameOver gameOver = null!;

    private readonly Dictionary<Type, object> dictionary = new();

    public T Get<T>(Type type) => (T) dictionary[type];

    public void Register<T>(T toRegister) => dictionary[typeof(T)] = toRegister;

}


public record GetDepedency(IDependencyCommand command): IEvent;

public interface DependencyProvider
{
    public T Get<T>(Type type);
    public void Register<T>(T item);

}
public interface IDependencyCommand
{
    public void HandleLocator(DependencyProvider prefabs);

}

public record GetterCommand<T>(Action<T> SetItem): IDependencyCommand
{
    public void HandleLocator(DependencyProvider prefabs)
    {
        SetItem(prefabs.Get<T>(typeof(T)));
    }

    public static T InvokeGet()
    {
        T item = default!;
        new GetDepedency(new GetterCommand<T>((gotten) => item = gotten)).Invoke();
        return item;
    }
}


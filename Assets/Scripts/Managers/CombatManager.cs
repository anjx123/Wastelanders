using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CombatManager : MonoBehaviour
{
    //Getter for the Singleton is found here
    public static CombatManager Instance { get; private set; }

    private GameState gameState;

    public CinemachineVirtualCamera baseCamera;
    public CinemachineVirtualCamera dynamicCamera;

    // Priority Queue
    public List<PlayerClass> players;
    public List<EnemyClass> enemies;

    public GameObject handContainer;
    public GameObject startDequeue;

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
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (!DependenciesReady())
        {
            yield return null;
        }
        GameState = GameState.SELECTION;
    }

    public bool DependenciesReady()
    {
        return enemies.Count == 3;
    }

    //Sets the Camera Center to the following Entity. 
    public void SetCameraCenter(EntityClass entity)
    {
        dynamicCamera.Follow = entity.transform;
        UpdateCameraBounds();
    }

    //Sets the Camera Bounds to "see more" in the direction that the following entity is facing
    //Usage: Should be called everytime an Entity changes their direction. 
    public void UpdateCameraBounds()
    {
        if (dynamicCamera.Follow.GetComponent<EntityClass>()?.IsFacingRight() ?? false)
        {
            var transposer = dynamicCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_ScreenX = 0.25f;
        }
        else if (!(dynamicCamera.Follow.GetComponent<EntityClass>()?.IsFacingRight()) ?? false)
        {
            var transposer = dynamicCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_ScreenX = 0.75f;
        }
    }
    
    private void PerformSelection()
    {
        startDequeue.SetActive(true);
        handContainer.SetActive(true);
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
        foreach (EnemyClass enemy in enemies)
        {
            enemy.AddAttack(players);
        }
    }

    private void PerformFighting()
    {
        startDequeue.SetActive(false);
        handContainer.SetActive(false);
        baseCamera.Priority = 0;
        dynamicCamera.Priority = 1;
    }

    public bool CanHighlight()
    {
        return GameState == GameState.SELECTION;
    }

    public GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            switch (gameState)
            {
                case GameState.SELECTION:
                    PerformSelection();
                    break;
                case GameState.FIGHTING:
                    PerformFighting();
                    break;
                default:
                    break;

            }
        }
    }
}

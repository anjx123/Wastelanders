using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //Getter for the Singleton is found here
    public static CombatManager Instance { get; private set; }

    private GameState gameState;

    public CinemachineVirtualCamera baseCamera;
    public CinemachineVirtualCamera dynamicCamera;

    // Priority Queue
    public List<EntityClass> players;
    public List<EntityClass> enemies;

    public GameObject instance; // Assign your instantiated object in the Inspector
    public GameObject container; // Assign your canvas in the Inspector

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
    void Start()
    {
        GameState = GameState.SELECTION;
    }
    
    // Update is called once per frame
    void Update()
    {

    }
    private void PerformSelection()
    {
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
    }

    private void PerformFighting()
    {
        baseCamera.Priority = 0;
        dynamicCamera.Priority = 1;
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

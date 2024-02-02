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

    private List<PlayerClass> players = new();
    private List<EnemyClass> enemies = new();

    public GameObject handContainer;
    public GameObject startDequeue;
    public GameObject combatUICardDisplay;
    [SerializeField]
    private SpriteRenderer fadeScreen;
    public string FADE_SORTING_LAYER
    {
        get
        {
            return fadeScreen.sortingLayerName;
        }
    }

    public int FADE_SORTING_ORDER
    {
        get
        {
            return fadeScreen.sortingOrder;
        }
    }

    public float FADE_SCREEN_Z_VALUE
    {
        get
        {
            return fadeScreen.gameObject.transform.position.z;
        }
    }


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

    }

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.SELECTION;
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
        if (dynamicCamera.Follow?.GetComponent<EntityClass>()?.IsFacingRight() ?? false)
        {
            var transposer = dynamicCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_ScreenX = 0.25f;
        }
        else if (!(dynamicCamera.Follow?.GetComponent<EntityClass>()?.IsFacingRight()) ?? false)
        {
            var transposer = dynamicCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_ScreenX = 0.75f;
        }
    }

    //Allows players to start selection again, resets enemies attacks and position
    private void PerformSelection()
    {
        Activate(startDequeue);
        Activate(handContainer);
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
        StartCoroutine(FadeBackground(false));

        // Each enemy declares an attack. players is passed to AddAttack so the enemy can choose a target.
        foreach (EnemyClass enemy in enemies)
        {
            enemy.AddAttack(players);
            StartCoroutine(enemy.ResetPosition());
        }

        foreach (PlayerClass player in players)
        {
            player.DrawToMax();
            StartCoroutine(player.ResetPosition());

        }

    }

    private void Activate(GameObject gameObject)
    {
        if (gameObject.GetComponent<Collider2D>())
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }
        Vector3 position = gameObject.GetComponent<Transform>().position;
        position.z = -1;
        gameObject.GetComponent<Transform>().position = position;
    }

    private void Deactivate(GameObject gameObject)
    {
        if (gameObject.GetComponent<Collider2D>())
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        Vector3 position = gameObject.GetComponent<Transform>().position;
        position.z = -200;
        gameObject.GetComponent<Transform>().position = position;
    }

    public void AddPlayer(PlayerClass player)
    {
        players.Add(player);
    }
    
    //Purpose: Call this when a player is removed or killed
    public void RemovePlayer(PlayerClass player)
    {
        if (players.Count > 0)
        {
            players.Remove(player);
            if (dynamicCamera.Follow?.GetComponent<PlayerClass>() == player)
            {
                dynamicCamera.Follow = null;
            }
            
        }
        if (players.Count == 0)
        {
            GameState = GameState.GAME_LOSE;
        }
    }
    
    public void AddEnemy(EnemyClass enemy)
    {
        enemies.Add(enemy);
    }
    //Purpose: Call this when an enemy is removed or killed
    public void RemoveEnemy(EnemyClass enemy)
    {
        if (enemies.Count > 0)
        {
            enemies.Remove(enemy);
            if (dynamicCamera.Follow?.GetComponent<EnemyClass>() == enemy)
            {
                dynamicCamera.Follow = null;
            }
        } 
        if (enemies.Count == 0)
        {
            GameState = GameState.GAME_WIN;
        }
    }

    private void PerformLose()
    {
        Debug.LogWarning("All Players are dead, You Lose...");
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
    }

    private void PerformWin()
    {
        Debug.LogWarning("All enemies are dead, You Win!");
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
    }



    private void PerformFighting()
    {
        CombatCardDisplayManager.Instance.HideCard();
        Deactivate(startDequeue);
        Deactivate(handContainer);
        baseCamera.Priority = 0;
        dynamicCamera.Priority = 1;
        StartCoroutine(FadeBackground(true));
    }

    private IEnumerator FadeBackground(bool darkenScene)
    {
        float startValue = fadeScreen.color.a;
        float elapsedTime = 0;
        float duration = 1f;

        float endValue;
        if (darkenScene)
        {
            endValue = 0.8f;
        }
        else
        {
            startValue = Mathf.Max(fadeScreen.color.a - 0.3f, 0f);
            endValue = 0f;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, newAlpha);
            yield return null;
        }
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
                case GameState.GAME_WIN:
                    PerformWin();
                    break;
                case GameState.GAME_LOSE:
                    PerformLose();
                    break;
                default:
                    break;

            }
        }
    }
}

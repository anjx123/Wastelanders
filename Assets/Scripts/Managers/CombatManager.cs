using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;
using System.Security.Cryptography;
using Systems.Persistence;

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
    public GameObject battleQueueParent;
    
    [SerializeField]
    private SpriteRenderer fadeScreen;
    bool fadeActive = false;

    [SerializeField] private PlayerDatabase playerDatabase;
    [SerializeField] private CardDatabase cardDatabase;

    public List<ActionClass> GetDeck(PlayerDatabase.PlayerName playerName)
    {
        return cardDatabase.ConvertStringsToCards(playerDatabase.GetDeckByPlayerName(playerName));  
    }

#nullable enable
    public delegate void GameStateChangedHandler(GameState newState); // Subscribe to this delegate if you want something to be run when gamestate changes
    public static event GameStateChangedHandler? OnGameStateChanged;
    public static event GameStateChangedHandler? OnGameStateChanging;

    public delegate void EntitiesWinLoseDelegate();
    public static event EntitiesWinLoseDelegate? PlayersWinEvent;
    public static event EntitiesWinLoseDelegate? EnemiesWinEvent;
    public string FADE_SORTING_LAYER
    {
        get
        {
            return fadeScreen.sortingLayerName;
        }
    }

    public int FADE_SORTING_LAYER_ID
    {
        get
        {
            return fadeScreen.sortingLayerID;
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

    public static void ClearEvents()
    {
        OnGameStateChanged = null;
        OnGameStateChanging = null;
        PlayersWinEvent = null;
        EnemiesWinEvent = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.GAME_START; //Put game start code in the performGameStart method.
        ActionClass.CardStateChange += HandleCrosshairEnemies;
    }

    private void OnDestroy()
    {
        ActionClass.CardStateChange -= HandleCrosshairEnemies;
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
        battleQueueParent.SetActive(true);
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
        StartCoroutine(FadeCombatBackground(false));
        SaveLoadSystem.Instance.LoadCardEvolutionProgress(); // Most universal place to put this is here, but tagged for performance optimizations

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

        if (players.Count > 0)
        {
            PlayerClass? jackie = players.FirstOrDefault(player => player is Jackie);
            if (jackie != null)
            {
                HighlightManager.Instance.SetActivePlayer(jackie);
            } else
            {
                HighlightManager.Instance.SetActivePlayer(players[0]);
            }
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
        players.Remove(player);
        if (dynamicCamera.Follow?.GetComponent<PlayerClass>() == player)
        {
            dynamicCamera.Follow = null;
        }

        if (players.Count == 0)
        {
            EnemiesWinEvent?.Invoke();
        }
    }
    
    public void AddEnemy(EnemyClass enemy)
    {
        enemies.Add(enemy);
    }

    //Purpose: Call this when an enemy is removed or killed
    public void RemoveEnemy(EnemyClass enemy)
    {
        enemies.Remove(enemy);
        if (dynamicCamera.Follow?.GetComponent<EnemyClass>() == enemy)
        {
            dynamicCamera.Follow = null;
        }

        bool allAreNeutral = enemies.OfType<NeutralEntityInterface>().Count() == enemies.Count;
        if (enemies.Count == 0 || allAreNeutral)
        {
            PlayersWinEvent?.Invoke();
        }
    }

    private void PerformLose()
    {
        StartCoroutine(FadeCombatBackground(false));
        Debug.LogWarning("All Players are dead, You Lose...");
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
        PerformOutOfCombat();
        //Save game after loss too.
        SaveLoadSystem.Instance.SaveGame();
    }

    private void PerformWin()
    {
        StartCoroutine(FadeCombatBackground(false));
        Debug.LogWarning("All enemies are dead, You Win!");
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
        // Save game after each win (including wiping out a wave) 
        SaveLoadSystem.Instance.SaveGame();
    }



    private void PerformFighting()
    {
        Deactivate(startDequeue);
        Deactivate(handContainer);
        baseCamera.Priority = 0;
        dynamicCamera.Priority = 1;
        StartCoroutine(FadeCombatBackground(true));
    }

    public void ActivateDynamicCamera()
    {
        baseCamera.Priority = 0;
        dynamicCamera.Priority = 1;
    }

    public void ActivateBaseCamera()
    {
        baseCamera.Priority = 1;
        dynamicCamera.Priority = 0;
    }

    private void PerformGameStart()
    {
        
    }

    public void SetDarkScreen()
    {
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
    }

    public IEnumerator FadeInLightScreen(float duration)
    {
        yield return StartCoroutine(FadeBackground(1f, 0f, duration));
    }

    public IEnumerator FadeInDarkScreen(float duration)
    {
        yield return StartCoroutine(FadeBackground(0f, 1f, duration));
    }
    private void PerformOutOfCombat()
    {
        Deactivate(handContainer);
        Deactivate(startDequeue);
        battleQueueParent.SetActive(false);

        foreach (PlayerClass player in players)
        {
            player.OutOfCombat();
            player.UnTargetable();
        }

        foreach (EnemyClass enemy in enemies)
        {
            enemy.OutOfCombat();
            enemy.UnTargetable();
        }
    }

    private void PerformInCombat()
    {
        foreach (PlayerClass player in players)
        {
            player.InCombat();
            player.Targetable();
        }

        foreach (EnemyClass enemy in enemies)
        {
            enemy.InCombat();
            enemy.Targetable();
        }
    }

    public void SetEnemiesPassive(List<EnemyClass> passiveEnemies)
    {
        enemies.RemoveAll(enemy => passiveEnemies.Contains(enemy));
        foreach (EnemyClass enemy in passiveEnemies)
        {
            enemy.OutOfCombat();
            enemy.UnTargetable();
        }
    }

    public void SetEnemiesHostile(List<EnemyClass> hostileEnemies)
    {
        enemies.AddRange(hostileEnemies);
        foreach (EnemyClass enemy in hostileEnemies)
        {
            enemy.InCombat();
            enemy.Targetable();
        }
    }

    //set (@param darkenScene) true to fade **combat background** in, false to fade out
    private IEnumerator FadeCombatBackground(bool darkenScene)
    {

        float startValue = fadeScreen.color.a;
        float duration = 1f;

        float endValue;
        if (darkenScene)
        {
            endValue = 0.8f;
        }
        else
        {
            startValue = Mathf.Max(fadeScreen.color.a - 0.3f, 0f); //Clamped to prevent visual nausua with strange alpha change
            endValue = 0f;
        }

        yield return StartCoroutine(FadeBackground(startValue, endValue, duration));
    }
    //Fade Background that gives you more control over the level of fade
    private IEnumerator FadeBackground(float startAlpha, float endAlpha, float duration)
    {
        if (fadeActive) yield break;
        fadeActive = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, newAlpha);
            yield return null;
        }
        fadeActive = false;
    }
    private void CrosshairAllEnemies()
    {
        foreach (EnemyClass enemy in enemies)
        {
            enemy.CrossHair();
        }
    }

    private void UncrosshairAllEnemies()
    {
        foreach (EnemyClass enemy in enemies)
        {
            enemy.UnCrossHair();
        }
    }

    private void HandleCrosshairEnemies(ActionClass.CardState previousState, ActionClass.CardState nextState)
    {
        if (nextState == ActionClass.CardState.CLICKED_STATE && previousState == ActionClass.CardState.HOVER)
        {
            CrosshairAllEnemies();
        }
        else if (nextState == ActionClass.CardState.HOVER && previousState == ActionClass.CardState.CLICKED_STATE)
        {
            UncrosshairAllEnemies();
        }
    }

    public bool CanHighlight()
    {
        return (PauseMenu.IsPaused) ? false : GameState == GameState.SELECTION;
    }

    public GameState GameState
    {
        get => gameState;
        set
        {
            OnGameStateChanging?.Invoke(value);
            gameState = value;
            switch (value)
            {
                case GameState.SELECTION:
                    PerformSelection(); //Gamestate no longer enters selection automatically and requires a scene object to manually start combat. 
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
                case GameState.GAME_START:
                    PerformGameStart(); //Careful, if you set the game state within these methods you can get strange behaviour
                    break;
                case GameState.OUT_OF_COMBAT:
                    PerformOutOfCombat();
                    break;
                default:
                    break;
            }
            OnGameStateChanged?.Invoke(value);
        }
    }

    public List<PlayerClass> GetPlayers()
    {
        return new List<PlayerClass>(players);
    }

    public List<EnemyClass> GetEnemies() 
    {
     return new List<EnemyClass>(enemies);
    }

}

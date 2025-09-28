using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//Game states enum
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Reward
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameConfiguration gameConfig;

    [Header("Current Game State")]
    public GameState currentState = GameState.MainMenu;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateGameSettings();
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start in main menu state
        ChangeGameState(GameState.MainMenu);
        SubscribeToStateEvents();
    }

    //On destroy to unsubscribe from events
    private void OnDestroy()
    {
        UnsubscribeFromStateEvents();
    }

    //Subscribe to state events
    private void SubscribeToStateEvents()
    {
        GameStateManager.OnStateChanged += OnStateChanged;
        GameDataManager.OnTimeExpired += OnTimeExpired;
    }

    //Unsubscribe from state events
    private void UnsubscribeFromStateEvents()
    {
        GameStateManager.OnStateChanged -= OnStateChanged;
        GameDataManager.OnTimeExpired -= OnTimeExpired;
    }

    //On state changed
    private void OnStateChanged(GameState newState)
    {
        currentState = newState;
    }

    //On time expired - called when game timer runs out
    private void OnTimeExpired()
    {
        EndGame();
    }



    //Validate game settings
    private void ValidateGameSettings()
    {
        if (gameConfig == null)
        {
            Debug.LogError("GameManager: GameConfiguration is not assigned! Using default values.");
            CreateDefaultGameConfig();
        }
    }

    //Create default game configuration if not assigned
    private void CreateDefaultGameConfig()
    {
        gameConfig = ScriptableObject.CreateInstance<GameConfiguration>();
        gameConfig.gameTime = Constants.DEFAULT_GAME_TIME;
        gameConfig.maxGameTime = Constants.DEFAULT_MAX_GAME_TIME;
        gameConfig.minGameTime = Constants.DEFAULT_MIN_GAME_TIME;
        gameConfig.perfectShotPoints = Constants.PERFECT_SHOT_POINTS;
        gameConfig.normalShotPoints = Constants.NORMAL_SHOT_POINTS;
        gameConfig.backboardBonusPoints = new int[] { Constants.DEFAULT_BONUS_POINTS_1, Constants.DEFAULT_BONUS_POINTS_2, Constants.DEFAULT_BONUS_POINTS_3 };
        gameConfig.bonusPeriodInterval = Constants.DEFAULT_BONUS_PERIOD_INTERVAL;
        gameConfig.bonusPeriodDuration = Constants.DEFAULT_BONUS_PERIOD_DURATION;
        gameConfig.bonusPeriodVariation = Constants.DEFAULT_BONUS_PERIOD_VARIATION;
        gameConfig.scoreThresholds = new int[] { Constants.DEFAULT_SCORE_THRESHOLD_1, Constants.DEFAULT_SCORE_THRESHOLD_2, Constants.DEFAULT_SCORE_THRESHOLD_3 };
    }

    //Initialize game with default values
    void InitializeGame()
    {   
        //Initialize game data
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetGameTime(gameConfig.gameTime);
        }
    }

    //Start new game and change state to playing
    public void StartNewGame()
    {
        try
        {
            // Reset game data
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.SetGameTime(gameConfig.gameTime);
                GameDataManager.Instance.ResetGameData();
            }

            //Load gameplay scene
            if (string.IsNullOrEmpty(Constants.GAMEPLAY_SCENE))
            {
                Debug.LogError("GameManager: Gameplay scene name is null or empty!");
                return;
            }

            SceneManager.LoadScene(Constants.GAMEPLAY_SCENE);
            ChangeGameState(GameState.Playing);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Failed to start new game: " + e.Message);
        }
    }

    //Pause game and change state to paused
    public void PauseGame()
    {
        if(currentState == GameState.Playing)
        {
            ChangeGameState(GameState.Paused);
        }
    }

    //Resume game and change state to playing
    public void ResumeGame()
    {
        if(currentState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
        }
    }

    //End game and change state to game over
    public void EndGame()
    {
        try
        {
            ChangeGameState(GameState.GameOver);

            if (string.IsNullOrEmpty(Constants.REWARD_SCENE))
            {
                Debug.LogError("GameManager: Reward scene name is null or empty!");
                return;
            }

            SceneManager.LoadScene(Constants.REWARD_SCENE);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Failed to end game: " + e.Message);
        }
    }

    //Back to main menu and change state to main menu
    public void BackToMainMenu()
    {
        try
        {
            ChangeGameState(GameState.MainMenu);

            if (string.IsNullOrEmpty(Constants.MAIN_MENU_SCENE))
            {
                Debug.LogError("GameManager: Main menu scene name is null or empty!");
                return;
            }

            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameManager: Failed to return to main menu: " + e.Message);
        }
    }



    //Change game state
    void ChangeGameState(GameState newState)
    {
        currentState = newState;
        
        // Update centralized state manager
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(newState);
        }
        
    }
}

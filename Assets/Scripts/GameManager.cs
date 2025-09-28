using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//Game states enum
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Reward
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Configuration is now handled by ConfigurationManager

    [Header("Current Game State")]
    public GameState currentState = GameState.MainMenu;
    private GameState previousState = GameState.MainMenu;


    // Events for state changes
    public static event Action<GameState> OnStateChanged;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        GameDataManager.OnTimeExpired += OnTimeExpired;
    }

    //Unsubscribe from state events
    private void UnsubscribeFromStateEvents()
    {
        GameDataManager.OnTimeExpired -= OnTimeExpired;
    }

    //On time expired - called when game timer runs out
    private void OnTimeExpired()
    {
        EndGame();
    }




    //Initialize game with default values
    void InitializeGame()
    {   
        //Initialize game data
        if (GameDataManager.Instance != null && ConfigurationManager.Instance != null)
        {
            GameDataManager.Instance.SetGameTime(ConfigurationManager.Instance.GetGameTime());
        }
    }

    //Start new game and change state to playing
    public void StartNewGame()
    {
        try
        {
            // Reset game data
            if (GameDataManager.Instance != null && ConfigurationManager.Instance != null)
            {
                GameDataManager.Instance.SetGameTime(ConfigurationManager.Instance.GetGameTime());
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

    //End game and go directly to reward
    public void EndGame()
    {
        try
        {
            if (string.IsNullOrEmpty(Constants.REWARD_SCENE))
            {
                Debug.LogError("GameManager: Reward scene name is null or empty!");
                return;
            }

            SceneManager.LoadScene(Constants.REWARD_SCENE);
            ChangeGameState(GameState.Reward);
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
        if (currentState == newState) return;

        previousState = currentState;
        currentState = newState;

        // Handle state-specific logic
        HandleStateTransition(previousState, currentState);

        // Notify listeners
        OnStateChanged?.Invoke(currentState);
    }

    //Handles state-specific transition logic
    private void HandleStateTransition(GameState fromState, GameState toState)
    {
        switch (toState)
        {
            case GameState.MainMenu:
                HandleMainMenuTransition(fromState);
                break;
            case GameState.Playing:
                HandlePlayingTransition(fromState);
                break;
            case GameState.Paused:
                HandlePausedTransition(fromState);
                break;
            case GameState.Reward:
                HandleRewardTransition(fromState);
                break;
        }
    }
    
    //Handles the main menu transition
    private void HandleMainMenuTransition(GameState fromState)
    {
        Time.timeScale = 1f;
        Debug.Log("GameManager: Transitioned to MainMenu from " + fromState);
    }
    
    //Handles the playing transition
    private void HandlePlayingTransition(GameState fromState)
    {
        if (fromState == GameState.Paused)
        {
            Time.timeScale = 1f;
        }
        Debug.Log("GameManager: Transitioned to Playing from " + fromState);
    }

    //Handles the paused transition
    private void HandlePausedTransition(GameState fromState)
    {
        if (fromState == GameState.Playing)
        {
            Time.timeScale = 0f;
        }
        Debug.Log("GameManager: Transitioned to Paused from " + fromState);
    }


    //Handles the reward transition
    private void HandleRewardTransition(GameState fromState)
    {
        Time.timeScale = 1f;
        Debug.Log("GameManager: Transitioned to Reward from " + fromState);
    }
}

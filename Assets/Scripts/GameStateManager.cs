using UnityEngine;
using System;

//Centralized state management system for the basketball game
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Game State")]
    //Current game state
    [SerializeField] private GameState currentState = GameState.MainMenu;
    [SerializeField] private GameState previousState = GameState.MainMenu;

    [Header("Game Data")]
    //Current game data
    [SerializeField] private GameData currentGameData;

    // Events for state changes
    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStateManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStateManager()
    {
        currentGameData = new GameData();
        ChangeState(GameState.MainMenu);
    }

    //Changes the current game state
    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        previousState = currentState;
        currentState = newState;

        // Handle state-specific logic
        HandleStateTransition(previousState, currentState);

        // Notify listeners
        OnStateChanged?.Invoke(currentState);
    }

    //Gets the current game state
    public GameState GetCurrentState()
    {
        return currentState;
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
            case GameState.GameOver:
                HandleGameOverTransition(fromState);
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
        Debug.Log("GameStateManager: Transitioned to MainMenu from " + fromState);
    }
    
    //Handles the playing transition
    private void HandlePlayingTransition(GameState fromState)
    {
        if (fromState == GameState.Paused)
        {
            Time.timeScale = 1f;
        }
    }

    //Handles the paused transition
    private void HandlePausedTransition(GameState fromState)
    {
        if (fromState == GameState.Playing)
        {
            Time.timeScale = 0f;
        }
        Debug.Log("GameStateManager: Transitioned to Paused from " + fromState);
    }

    //Handles the game over transition
    private void HandleGameOverTransition(GameState fromState)
    {
        Time.timeScale = 1f;
        Debug.Log("GameStateManager: Transitioned to GameOver from " + fromState);
    }

    //Handles the reward transition
    private void HandleRewardTransition(GameState fromState)
    {
        Time.timeScale = 1f;
        Debug.Log("GameStateManager: Transitioned to Reward from " + fromState);
    }
}

//Data structure containing current game information
[System.Serializable]
public class GameData
{
    [Header("Game Progress")]
    public float remainingTime = 60f;
    public int currentScore = 0;
    public int totalShots = 0;
    public int successfulShots = 0;
    public int perfectShots = 0;
    public int backboardBonuses = 0;

    [Header("Game Settings")]
    public float gameTime = 60f;
    public bool isBonusPeriodActive = false;
    public int currentBonusPoints = 0;

    [Header("Ball State")]
    public bool isBallInFlight = false;
    public bool hasBallScored = false;

    //Calculates and returns the current accuracy percentage
    public float GetAccuracy()
    {
        return totalShots > 0 ? (float)successfulShots / totalShots * 100f : 0f;
    }

    //Resets all game data to initial values
    public void Reset()
    {
        remainingTime = gameTime;
        currentScore = 0;
        totalShots = 0;
        successfulShots = 0;
        perfectShots = 0;
        backboardBonuses = 0;
        isBonusPeriodActive = false;
        currentBonusPoints = 0;
        isBallInFlight = false;
        hasBallScored = false;
    }

}

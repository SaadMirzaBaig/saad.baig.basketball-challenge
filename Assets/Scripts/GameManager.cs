using UnityEngine;
using UnityEngine.SceneManagement;
using System;


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

    [Header("Game Settings")]
    public float gameTime = 60f; // 60 seconds per game


    [Header("Current Game State")]
    public GameState currentState = GameState.MainMenu;
    public float remainingTime;


    // Events for UI updates
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<float> OnTimeUpdated;
    public static event Action OnGameFinished;

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
    }


    // Update is called once per frame
    void Update()
    {
        if(currentState == GameState.Playing)
        {
            UpdateGameTimer();
        }
    }

    void InitializeGame()
    {   
        //Initialize default values
        remainingTime = gameTime;
    }

    public void StartNewGame()
    {
        //Reset  value
        remainingTime = gameTime;

        //Load gameplay scene
        SceneManager.LoadScene("GamePlay");
        ChangeGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        if(currentState == GameState.Playing)
        {
            ChangeGameState(GameState.Paused);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if(currentState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
            Time.timeScale = 1f;
        }
    }

    public void EndGame()
    {
        ChangeGameState(GameState.GameOver);
        Time.timeScale = 1f;

        SceneManager.LoadScene("Reward");
        OnGameFinished?.Invoke();
    }

    public void BackToMainMenu()
    {
        ChangeGameState(GameState.MainMenu);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void UpdateGameTimer()
    {
        remainingTime -= Time.deltaTime;
        OnTimeUpdated?.Invoke(remainingTime);
        
        if(remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame();
        }
    }

    void ChangeGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(currentState);
    }

    public bool IsPlaying() => currentState == GameState.Playing;
    public bool IsPaused() => currentState == GameState.Paused;
    public bool IsGameActive() => currentState == GameState.Playing || currentState == GameState.Paused;
}

using UnityEngine;
using System;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private GameData currentGameData;
    private bool isTimerRunning = false;
    private int lastTimeSecond = -1; // Track last second to avoid spam

    // Events for Game data changes
    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnTimeChanged;
    public static event Action OnTimeExpired; // When game time runs out

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDataManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Initialize the data manager
    private void InitializeDataManager()
    {
        currentGameData = new GameData();
        SubscribeToStateEvents();
    }

    //Update timer when game is playing
    private void Update()
    {
        if (isTimerRunning && currentGameData.remainingTime > 0)
        {
            currentGameData.remainingTime -= Time.deltaTime;
            
            // Only notify when time hits a new second
            int currentSecond = Mathf.CeilToInt(currentGameData.remainingTime);
            if (currentSecond != lastTimeSecond)
            {
                lastTimeSecond = currentSecond;
                OnTimeChanged?.Invoke(currentGameData.remainingTime);
            }
            
            if (currentGameData.remainingTime <= 0)
            {
                currentGameData.remainingTime = 0;
                isTimerRunning = false;
                OnTimeExpired?.Invoke(); // Notify that time has run out
            }
        }
    }

    //Subscribe to state events
    private void SubscribeToStateEvents()
    {
        GameManager.OnStateChanged += OnGameStateChanged;
    }

    //On destroy to unsubscribe from events
    private void OnDestroy()
    {
        UnsubscribeFromStateEvents();
    }

    //Unsubscribe from state events
    private void UnsubscribeFromStateEvents()
    {
        GameManager.OnStateChanged -= OnGameStateChanged;
    }

    //Reset game data when game state changed
    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Playing)
        {
            ResetGameData();
            isTimerRunning = true;
            lastTimeSecond = -1; // Reset time tracking
        }
        else
        {
            isTimerRunning = false;
        }
    }


    //Get the current game data
    public GameData GetCurrentGameData()
    {
        return currentGameData;
    }

    //Reset all game data to initial values
    public void ResetGameData()
    {
        currentGameData.Reset();
        
    }

    //Update the remaining game time
    public void UpdateRemainingTime(float time)
    {
        if (currentGameData.remainingTime != time)
        {
            currentGameData.remainingTime = time;
            OnTimeChanged?.Invoke(time);
        }
    }

    //Update the current score
    public void UpdateScore(int score)
    {
        if (currentGameData.currentScore != score)
        {
            currentGameData.currentScore = score;
            OnScoreChanged?.Invoke(score);
        }
    }

    //Update shot statistics
    public void UpdateShotStats(int successfulShots, int totalShots)
    {
        
        if (currentGameData.successfulShots != successfulShots)
        {
            currentGameData.successfulShots = successfulShots;
        }
        
        if (currentGameData.totalShots != totalShots)
        {
            currentGameData.totalShots = totalShots;
        }


    }

    //Update perfect shot count
    public void UpdatePerfectShots(int perfectShots)
    {
        if (currentGameData.perfectShots != perfectShots)
        {
            currentGameData.perfectShots = perfectShots;
        }
    }

    //Update backboard bonus count
    public void UpdateBackboardBonuses(int backboardBonuses)
    {
        if (currentGameData.backboardBonuses != backboardBonuses)
        {
            currentGameData.backboardBonuses = backboardBonuses;
        }
    }

    //Update ball flight state
    public void UpdateBallState(bool isInFlight, bool hasScored = false)
    {
        
        if (currentGameData.isBallInFlight != isInFlight)
        {
            currentGameData.isBallInFlight = isInFlight;
        }
        
        if (currentGameData.hasBallScored != hasScored)
        {
            currentGameData.hasBallScored = hasScored;
        }


    }

    //Update bonus period state
    public void UpdateBonusPeriodState(bool isActive, int bonusPoints = 0)
    {
        
        if (currentGameData.isBonusPeriodActive != isActive)
        {
            currentGameData.isBonusPeriodActive = isActive;
        }
        
        if (currentGameData.currentBonusPoints != bonusPoints)
        {
            currentGameData.currentBonusPoints = bonusPoints;        }

    }

    //Set the game time duration
    public void SetGameTime(float gameTime)
    {
        if (currentGameData.gameTime != gameTime)
        {
            currentGameData.gameTime = gameTime;
            currentGameData.remainingTime = gameTime;
        }
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

using UnityEngine;
using System;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameConfiguration gameConfig;

    //Bonus period management
    [Header ("Bonus Period")]
    //Is bonus period active
    private bool isBonusPeriodActive = false;
    private Coroutine bonusPeriodCoroutine;
    private int currentBonusPoints = 0; // Store the current bonus period's points

    public static event Action<int> OnBackboardBonus;
    public static event Action<int> OnBonusPeriodStarted; // Now passes the bonus points
    public static event Action OnBonusPeriodEnded;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SubscribeToStateEvents(); //Subscribe to state events
        StartBonusPeriodCycle(); //Start bonus period cycle
    }

    //Subscribe to state events
    private void SubscribeToStateEvents()
    {
        GameStateManager.OnStateChanged += OnStateChanged;
    }

    //Unsubscribe from state events
    private void UnsubscribeFromStateEvents()
    {
        GameStateManager.OnStateChanged -= OnStateChanged;
    }

    //Reset and start/stop bonus period cycle when playing
    private void OnStateChanged(GameState newState)
    {
        if (newState == GameState.Playing)
        {
            ResetScore();
            StartBonusPeriodCycle();
        }
        else if (newState == GameState.GameOver || newState == GameState.Paused)
        {
            StopBonusPeriodCycle();
        }
    }

 

    //On destroy to unsubscribe from events
    private void OnDestroy()
    {
        UnsubscribeFromStateEvents();
    }


    //Start bonus period cycle
    private void StartBonusPeriodCycle()
    {
        if (bonusPeriodCoroutine != null)
        {
            StopCoroutine(bonusPeriodCoroutine);
        }
        bonusPeriodCoroutine = StartCoroutine(BonusPeriodCycle());
    }

    private void StopBonusPeriodCycle()
    {
        if (bonusPeriodCoroutine != null)
        {
            StopCoroutine(bonusPeriodCoroutine);
            bonusPeriodCoroutine = null;
        }
        isBonusPeriodActive = false;
    }

    private IEnumerator BonusPeriodCycle()
    {
        while (true)
        {
            // Wait for the interval (with random variation)
            float waitTime = gameConfig.bonusPeriodInterval + UnityEngine.Random.Range(0f, gameConfig.bonusPeriodVariation);
            yield return new WaitForSeconds(waitTime);

            // Generate random bonus points for this period
            currentBonusPoints = gameConfig.backboardBonusPoints[UnityEngine.Random.Range(0, gameConfig.backboardBonusPoints.Length)];

            // Start bonus period
            isBonusPeriodActive = true;
            OnBonusPeriodStarted?.Invoke(currentBonusPoints);

            // Wait for bonus period duration
            yield return new WaitForSeconds(gameConfig.bonusPeriodDuration);

            // End bonus period
            isBonusPeriodActive = false;
            OnBonusPeriodEnded?.Invoke();
        }
    }

    public void ResetScore()
    {
        // Reset centralized data
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetGameData();
        }
    }

    //Register shot with bool checks for successful, perfect, and backboard bonus
    //Add score based on shot type
    public void RegisterShot(bool isSuccessful, bool isPerfect = false, bool hasBackboardBonus = false)
    {
        if (GameDataManager.Instance == null) return;

        GameData currentData = GameDataManager.Instance.GetCurrentGameData();
        
        // Update shot count
        GameDataManager.Instance.UpdateShotStats(
            currentData.successfulShots + (isSuccessful ? 1 : 0),
            currentData.totalShots + 1
        );

        if (isSuccessful)
        {
            int points = isPerfect ? gameConfig.perfectShotPoints : gameConfig.normalShotPoints;

            if (isPerfect)
            {
                GameDataManager.Instance.UpdatePerfectShots(currentData.perfectShots + 1);
            }

            if (hasBackboardBonus && isBonusPeriodActive)
            {
                points += currentBonusPoints;
                GameDataManager.Instance.UpdateBackboardBonuses(currentData.backboardBonuses + 1);
                OnBackboardBonus?.Invoke(currentBonusPoints);
            }

            AddScore(points);
        }
    }

    //Add score to current score
    private void AddScore(int points)
    {
        if (GameDataManager.Instance != null)
        {
            GameData currentData = GameDataManager.Instance.GetCurrentGameData();
            GameDataManager.Instance.UpdateScore(currentData.currentScore + points);
        }
    }

    //Get accuracy from GameDataManager
    public float GetAccuracy()
    {
        if (GameDataManager.Instance != null)
        {
            GameData currentData = GameDataManager.Instance.GetCurrentGameData();
            return currentData.GetAccuracy();
        }
        return 0f;
    }

    //Get final stats from GameDataManager
    public GameData GetFinalStats()
    {
        return GameDataManager.Instance != null ? 
        GameDataManager.Instance.GetCurrentGameData() : new GameData();
    }
}

using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance { get; private set; }

    [Header("Scoring Settings")]
    public int perfectShotPoints = 3;
    public int normalShotPoints = 2;
    public int[] backboardBonusPoints = { 4, 6, 8 };

    [Header ("Game Stats")]
    public int currentScore = 0;
    public int totalShots = 0;
    public int successfulShots = 0;
    public int perfectShots = 0;
    public int backboardBonuses = 0;

    public static event Action<int> OnScoreUpdated;
    public static event Action<int, int> OnShotStatsUpdated; // successful, total
    public static event Action<int> OnBackboardBonus;


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
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Playing)
        {
            ResetScore();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        totalShots = 0;
        successfulShots = 0;
        perfectShots = 0;
        backboardBonuses = 0;

        OnScoreUpdated?.Invoke(currentScore);
        OnShotStatsUpdated?.Invoke(successfulShots, totalShots);

    }

    public void RegisterShot(bool isSuccessful, bool isPerfect = false, bool hasBackboardBonus = false)
    {
        totalShots++;

        if (isSuccessful)
        {
            successfulShots++;

            int points = isPerfect ? perfectShotPoints : normalShotPoints;

            if (isPerfect)
            {
                perfectShots++;
            }

            if (hasBackboardBonus)
            {
                int bonusPoints = backboardBonusPoints[UnityEngine.Random.Range(0, backboardBonusPoints.Length)];
                points += bonusPoints;
                backboardBonuses++;
                OnBackboardBonus?.Invoke(bonusPoints);
            }

            AddScore(points);
        }

        OnShotStatsUpdated?.Invoke(successfulShots, totalShots);

    }

    private void AddScore(int points)
    {
        currentScore += points;
        OnScoreUpdated?.Invoke(currentScore);
    }

    public float GetAccuracy()
    {
        return totalShots > 0 ? (float)successfulShots / totalShots * 100f : 0f;
    }

    public GameStats GetFinalStats()
    {
        return new GameStats
        {
            finalScore = currentScore,
            totalShots = totalShots,
            successfulShots = successfulShots,
            perfectShots = perfectShots,
            backboardBonuses = backboardBonuses,
            accuracy = GetAccuracy()
        };
    }
}

[System.Serializable]
public struct GameStats
{
    public int finalScore;
    public int totalShots;
    public int successfulShots;
    public int perfectShots;
    public int backboardBonuses;
    public float accuracy;
}

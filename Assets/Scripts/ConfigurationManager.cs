using UnityEngine;

// Centralized configuration manager that provides access to game configuration
public class ConfigurationManager : MonoBehaviour
{
    public static ConfigurationManager Instance { get; private set; }

    [Header("Game Configuration")]
    [SerializeField] private GameConfiguration gameConfig;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateConfiguration();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Validate configuration and create default if needed
    private void ValidateConfiguration()
    {
        if (gameConfig == null)
        {
            Debug.LogError("ConfigurationManager: GameConfiguration is not assigned! Using default values.");
            CreateDefaultConfiguration();
        }
    }

    //Create default game configuration if not assigned
    private void CreateDefaultConfiguration()
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

    //Get the game configuration
    public GameConfiguration GetGameConfiguration()
    {
        return gameConfig;
    }

    //Get specific configuration values
    public float GetGameTime() => gameConfig.gameTime;
    public int GetPerfectShotPoints() => gameConfig.perfectShotPoints;
    public int GetNormalShotPoints() => gameConfig.normalShotPoints;
    public int[] GetBackboardBonusPoints() => gameConfig.backboardBonusPoints;
    public float GetBonusPeriodInterval() => gameConfig.bonusPeriodInterval;
    public float GetBonusPeriodDuration() => gameConfig.bonusPeriodDuration;
    public float GetBonusPeriodVariation() => gameConfig.bonusPeriodVariation;
    public int[] GetScoreThresholds() => gameConfig.scoreThresholds;
}

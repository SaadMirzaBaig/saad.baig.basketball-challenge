using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Basketball/Game Configuration")]
public class GameConfiguration : ScriptableObject
{
    [Header("Game Timing")]
    //Duration of game in seconds
    public float gameTime = 60f;
    
    //Maximum game time allowed
    public float maxGameTime = 300f;

    //Minimum game time allowed
    public float minGameTime = 10f;

    [Header("Scoring Settings")]
    //Points awarded for perfect shots
    public int perfectShotPoints = 3;

    //Points awarded for normal shots
    public int normalShotPoints = 2;

    //Bonus points for backboard hits during bonus periods
    public int[] backboardBonusPoints = { 4, 6, 8 };

    [Header("Bonus Period Settings")]
    //Time between bonus periods in seconds
    public float bonusPeriodInterval = 5f;
    
    //Duration of bonus period in seconds
    public float bonusPeriodDuration = 4f;
    
    //Random variation in bonus period interval
    public float bonusPeriodVariation = 1f;

    [Header("Star Rating Thresholds")]
    //Score thresholds for star ratings
    public int[] scoreThresholds = { 20, 50, 100 };

    [Header("Validation")]
    //Enable validation warnings
    public bool enableValidation = true;

    // Validates the configuration values
    private void OnValidate()
    {
        if (!enableValidation) return;

        // Validate game time
        if (gameTime < minGameTime)
        {
            Debug.LogWarning($"GameConfiguration: Game time is below minimum ({minGameTime}s). Setting to minimum.");
            gameTime = minGameTime;
        }
        else if (gameTime > maxGameTime)
        {
            Debug.LogWarning($"GameConfiguration: Game time exceeds maximum ({maxGameTime}s). Setting to maximum.");
            gameTime = maxGameTime;
        }

        // Validate scoring points
        if (perfectShotPoints <= 0)
        {
            Debug.LogWarning("GameConfiguration: Perfect shot points must be greater than 0. Setting to 3.");
            perfectShotPoints = Constants.PERFECT_SHOT_POINTS;
        }

        if (normalShotPoints <= 0)
        {
            Debug.LogWarning("GameConfiguration: Normal shot points must be greater than 0. Setting to 2.");
            normalShotPoints = Constants.NORMAL_SHOT_POINTS;
        }

        // Validate bonus points array
        if (backboardBonusPoints == null || backboardBonusPoints.Length == 0)
        {
            Debug.LogWarning("GameConfiguration: Backboard bonus points array is empty. Setting default values.");
            backboardBonusPoints = new int[] { Constants.DEFAULT_BONUS_POINTS_1, Constants.DEFAULT_BONUS_POINTS_2, Constants.DEFAULT_BONUS_POINTS_3 };
        }

        for (int i = 0; i < backboardBonusPoints.Length; i++)
        {
            if (backboardBonusPoints[i] <= 0)
            {
                Debug.LogWarning($"GameConfiguration: Bonus point at index {i} must be greater than 0. Setting to 4.");
                backboardBonusPoints[i] = Constants.DEFAULT_BONUS_POINTS_1;
            }
        }

        // Validate bonus period settings
        if (bonusPeriodInterval <= 0f)
        {
            Debug.LogWarning("GameConfiguration: Bonus period interval must be greater than 0. Setting to 5s.");
            bonusPeriodInterval = Constants.DEFAULT_BONUS_PERIOD_INTERVAL;
        }

        if (bonusPeriodDuration <= 0f)
        {
            Debug.LogWarning("GameConfiguration: Bonus period duration must be greater than 0. Setting to 4s.");
            bonusPeriodDuration = Constants.DEFAULT_BONUS_PERIOD_DURATION;
        }

        if (bonusPeriodVariation < 0f)
        {
            Debug.LogWarning("GameConfiguration: Bonus period variation cannot be negative. Setting to 0.");
            bonusPeriodVariation = Constants.DEFAULT_BONUS_PERIOD_VARIATION;
        }

        // Validate star rating thresholds
        if (scoreThresholds == null || scoreThresholds.Length == 0)
        {
            Debug.LogWarning("GameConfiguration: Score thresholds array is empty. Setting default values.");
            scoreThresholds = new int[] { Constants.DEFAULT_SCORE_THRESHOLD_1, Constants.DEFAULT_SCORE_THRESHOLD_2, Constants.DEFAULT_SCORE_THRESHOLD_3 };
        }

        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (scoreThresholds[i] < 0)
            {
                Debug.LogWarning($"GameConfiguration: Score threshold at index {i} cannot be negative. Setting to 0.");
                scoreThresholds[i] = 0;
            }
        }
    }

}

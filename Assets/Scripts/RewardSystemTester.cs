using UnityEngine;

// A simple test script to help debug the reward system
// Add this to any GameObject in your Reward scene to test star display
public class RewardSystemTester : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool runTestOnStart = true;
    
    void Start()
    {
        if (runTestOnStart)
        {
            TestRewardSystem();
        }
    }
    
    [ContextMenu("Test Reward System")]
    public void TestRewardSystem()
    {
        Debug.Log("=== REWARD SYSTEM TEST ===");
        
        // Test GameManager state
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }
        Debug.Log($"Current game state: {GameManager.Instance.currentState}");
        
        // Test ConfigurationManager
        if (ConfigurationManager.Instance == null)
        {
            Debug.LogError("ConfigurationManager.Instance is null!");
            return;
        }
        
        // Test ScoreThresholds
        int[] thresholds = ConfigurationManager.Instance.GetScoreThresholds();
        Debug.Log($"Score thresholds: [{string.Join(", ", thresholds)}]");
        
        // Test GameDataManager
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager.Instance is null!");
            return;
        }
        
        // Test ScoreManager
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager.Instance is null!");
            return;
        }
        
        // Get current game data
        GameData currentData = GameDataManager.Instance.GetCurrentGameData();
        Debug.Log($"Current game data:");
        Debug.Log($"  Score: {currentData.currentScore}");
        Debug.Log($"  Total Shots: {currentData.totalShots}");
        Debug.Log($"  Successful Shots: {currentData.successfulShots}");
        Debug.Log($"  Perfect Shots: {currentData.perfectShots}");
        Debug.Log($"  Backboard Bonuses: {currentData.backboardBonuses}");
        Debug.Log($"  Accuracy: {currentData.GetAccuracy():F1}%");
        
        // Calculate expected stars
        int earnedStars = 0;
        for (int i = 0; i < thresholds.Length; i++)
        {
            if (currentData.currentScore >= thresholds[i])
            {
                earnedStars = i + 1;
            }
        }
        
        Debug.Log($"Expected stars earned: {earnedStars}");
        Debug.Log("=== END TEST ===");
    }
    
    [ContextMenu("Simulate High Score")]
    public void SimulateHighScore()
    {
        Debug.Log("Simulating high score game...");
        
        if (GameDataManager.Instance != null)
        {
            // Simulate a high-scoring game
            GameDataManager.Instance.UpdateScore(35);
            GameDataManager.Instance.UpdateShotStats(15, 20);
            GameDataManager.Instance.UpdatePerfectShots(8);
            GameDataManager.Instance.UpdateBackboardBonuses(3);
            
            Debug.Log("High score simulation complete. Check reward UI!");
        }
    }
}

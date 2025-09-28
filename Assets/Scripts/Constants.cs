public static class Constants
{
    // Tags
    public const string BALL_TAG = "Ball";
    public const string RING_TAG = "Hoop";
    public const string BACKBOARD_TAG = "Backboard";

    // Scene names
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const string GAMEPLAY_SCENE = "Gameplay";
    public const string REWARD_SCENE = "Reward";

    // Game Constants
    public const float DEFAULT_GAME_TIME = 60f;
    public const float DEFAULT_MAX_GAME_TIME = 300f;
    public const float DEFAULT_MIN_GAME_TIME = 10f;

    // Ball Physics Constants
    public const float DEFAULT_BALL_MASS = 0.6f;
    public const float DEFAULT_BALL_RADIUS = 0.12f;
    public const float DEFAULT_BALL_DRAG = 0f;
    public const float DEFAULT_MAX_FLIGHT_DURATION = 2.5f;
    public const float DEFAULT_MIN_Y_RESET_THRESHOLD = -2f;

    // Basket Detection Constants
    public const float HOOP_RADIUS = 0.23f;
    public const float DETECTION_HEIGHT = 0.5f;
    public const float DETECTION_RADIUS_MULTIPLIER = 1.1f; // Slightly larger than hoop for reliable detection

  
    // Shot Physics Constants
    public const float MIN_FLIGHT_TIME = 0.5f;

    // Scoring Constants
    public const int PERFECT_SHOT_POINTS = 3;
    public const int NORMAL_SHOT_POINTS = 2;
    public const int DEFAULT_BONUS_POINTS_1 = 4;
    public const int DEFAULT_BONUS_POINTS_2 = 6;
    public const int DEFAULT_BONUS_POINTS_3 = 8;
    public const int DEFAULT_SCORE_THRESHOLD_1 = 20;
    public const int DEFAULT_SCORE_THRESHOLD_2 = 50;
    public const int DEFAULT_SCORE_THRESHOLD_3 = 100;
    public const int DEFAULT_BONUS_PERIOD_INTERVAL = 5;
    public const int DEFAULT_BONUS_PERIOD_DURATION = 4;
    public const int DEFAULT_BONUS_PERIOD_VARIATION = 1;

    // Validation Constants
    public const float MAX_BALL_MASS = 1f;
    public const float MAX_BALL_RADIUS = 0.5f;

}

using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [Header("UI Buttons")]
    public Button playButton;
    public Button exitButton;


    private void Start()
    {
        SetupButtons();
    }


    private void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked");
        GameManager.Instance?.StartNewGame();

    }

    private void OnExitClicked()
    {
        Debug.Log("Exit button clicked");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}

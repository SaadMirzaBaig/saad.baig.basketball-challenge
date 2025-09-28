using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{

    [Header("UI Buttons")]
    public Button playButton;
    public Button exitButton;


    private void Start()
    {
        SetupButtons();
    }

    //Setup all UI buttons
    private void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    //On play clicked
    private void OnPlayClicked()
    {
        GameManager.Instance?.StartNewGame();

    }

    //On exit clicked for editor and build
    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }


}

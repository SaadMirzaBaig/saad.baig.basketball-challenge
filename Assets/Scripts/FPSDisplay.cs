using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    [Header("FPS Display")]
    public TMP_Text fpsText;
    
    [Header("FPS Settings")]
    public float fpsUpdateInterval = 0.5f;
    public int targetFrameRate = 60;
    
    // FPS calculation variables
    private float fpsAccumulator = 0f;
    private int fpsFrames = 0;
    private float fpsTimeLeft;
    private float currentFPS;

    void Start()
    {
        InitializeFPS();
    }

    void Update()
    {
        UpdateFPS();
    }

    private void InitializeFPS()
    {
        Application.targetFrameRate = targetFrameRate;
        fpsTimeLeft = fpsUpdateInterval;
        
        if (fpsText != null)
        {
            fpsText.text = "FPS: --";
        }
    }

    private void UpdateFPS()
    {
        if (fpsText == null) return;

        fpsTimeLeft -= Time.deltaTime;
        fpsAccumulator += Time.timeScale / Time.deltaTime;
        fpsFrames++;

        if (fpsTimeLeft <= 0.0f)
        {
            currentFPS = fpsAccumulator / fpsFrames;
            fpsText.text = "FPS: " + Mathf.RoundToInt(currentFPS);
            
            fpsTimeLeft = fpsUpdateInterval;
            fpsAccumulator = 0.0f;
            fpsFrames = 0;
        }
    }
}

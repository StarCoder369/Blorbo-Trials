using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public InputActionReference pauseInput;

    public List<Transform> playerSpawnPoints;
    public int currentLevel;
    public List<CinemachineCamera> cameras;
    public List<string> trialName;
    public CinemachineCamera selectedCam;
    public TMP_Text trialNameTxt;
    public TMP_Text timeTxt;
    public TMP_Text deathsTxt;
    public TMP_Text endScreenTimeTxt;
    public TMP_Text endScreenDeathTxt;
    public TMP_Text endScreenScoreTxt;
    public float timeElapsed;
    public int deaths;
    public GameObject endScreen;
    public GameObject pauseScreen;
    public bool timerStarted = false;
    public bool gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        if (pauseInput != null)
        {
            pauseInput.action.Enable();
            pauseInput.action.performed += TogglePauseScreen;
        }
    }

    void OnDisable()
    {
        if (pauseInput != null)
        {
            pauseInput.action.performed -= TogglePauseScreen;
            pauseInput.action.Disable();
        }
    }

    void Start()
    {
        DisableAllCameras();
        EnableCamera(cameras[0]);
        UpdateTimerText();
        UpdateDeathsText();
        UpdateEndScreenText();
        Time.timeScale = 1;
    }

    void Update()
    {
        trialNameTxt.text = $"Trial {currentLevel + 1}: {trialName[currentLevel]}";

        if (timerStarted && !gameEnded)
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerText();
        }
    }

    void TogglePauseScreen(InputAction.CallbackContext context)
    {
        if (gameEnded)
        {
            return;
        }

        if (pauseScreen.activeSelf)
        {
            DisablePauseScreen();
        }
        else
        {
            EnablePauseScreen();
        }
    }

    public void EnablePauseScreen()
    {
        if (gameEnded)
        {
            return;
        }

        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void DisablePauseScreen()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    void UpdateTimerText()
    {
        TimeSpan time = TimeSpan.FromSeconds(timeElapsed);
        timeTxt.text = $"{(int)time.TotalMinutes}:{time.Seconds:D2}.{time.Milliseconds:D3}";
    }

    void UpdateDeathsText()
    {
        deathsTxt.text = $"Deaths: {deaths}";
    }

    void UpdateEndScreenText()
    {
        TimeSpan time = TimeSpan.FromSeconds(timeElapsed);
        endScreenTimeTxt.text = $"Your Time: {(int)time.TotalMinutes}:{time.Seconds:D2}.{time.Milliseconds:D3}";
        endScreenDeathTxt.text = $"Death Count: {deaths}";
        // Need to add +1 to avoid division by zero, or 0 and 1 death being the same.
        endScreenScoreTxt.text = $"Final Score: {time.TotalSeconds / (deaths + 1):F2}";
    }

    public void AddDeath()
    {
        deaths++;
        UpdateDeathsText();
    }

    public void ShowEndScreen()
    {
        endScreen.SetActive(true);
        UpdateEndScreenText();
        Time.timeScale = 0f;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        UpdateTimerText();
    }

    public void DisableAllCameras()
    {
        foreach (CinemachineCamera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }
    }

    public void EnableCamera(CinemachineCamera camera)
    {
        foreach (CinemachineCamera cam in cameras)
        {
            if (cam == camera)
            {
                cam.gameObject.SetActive(true);
                selectedCam = camera;
                return;
            }
        }
    }

    public void CompleteGame()
    {
        gameEnded = true;
        ShowEndScreen();
    }
}
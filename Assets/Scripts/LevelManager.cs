using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

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
    public float timeElapsed;
    public int deaths;
    public GameObject endScreen;
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

    void Start()
    {
        DisableAllCameras();
        EnableCamera(cameras[0]);
        UpdateTimerText();
        UpdateDeathsText();
        UpdateEndScreenText();
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
        Time.timeScale = 0;
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
        ShowEndScreen();
    }
}
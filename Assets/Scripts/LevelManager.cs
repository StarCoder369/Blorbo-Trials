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
    }

    void Update()
    {
        trialNameTxt.text = $"Trial {currentLevel + 1}: {trialName[currentLevel]}";
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
}

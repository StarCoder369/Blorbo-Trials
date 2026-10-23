using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<CinemachineCamera> cameras;

    public CinemachineCamera selectedCam;

    public static CameraManager Instance { get; private set; }

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
        cameras = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
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

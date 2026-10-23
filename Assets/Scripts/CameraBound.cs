using Unity.Cinemachine;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    public bool boundsEnabled = true;
    public CinemachineCamera cameraObj;
    public CinemachineCamera secondaryCameraObj;

    public Transform playerPos;
    public Transform secondaryPlayerPos;
}

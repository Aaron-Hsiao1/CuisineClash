using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { ThirdPerson, GoKart }

    private CameraMode currentMode;

    public void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;

        if (mode == CameraMode.ThirdPerson)
        {
            // Set up third-person camera behavior
            transform.position = new Vector3(0, 10, -10);
            transform.rotation = Quaternion.Euler(45, 0, 0);
        }
        else if (mode == CameraMode.GoKart)
        {
            // Set up GoKart camera behavior
            transform.position = new Vector3(0, 5, -5);
            transform.rotation = Quaternion.Euler(30, 0, 0);
        }
    }
}
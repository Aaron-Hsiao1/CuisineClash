using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IndicatorAimer : MonoBehaviour
{
     public Camera playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        
        // Set the direction to face the camera on the X and Z axes
        Vector3 targetDirection = new Vector3(cameraForward.x, cameraForward.y, cameraForward.z).normalized;
        
        // Smoothly rotate the object to face the target direction
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 50f); // Adjust the speed factor
        }
    }
}

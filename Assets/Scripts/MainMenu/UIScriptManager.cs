using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UIScriptManager : MonoBehaviour
{
    // Start is called before the first frame update

    public CinemachineVirtualCamera currentCamera;
    void Start()
    {
        currentCamera.Priority++;
    }

    // Update is called once per frame
    public void UpdateCamera(CinemachineVirtualCamera target)
    {
        currentCamera.Priority--;
        currentCamera = target;
        currentCamera.Priority++;
    }
    void Update()
    {
        
    }
}

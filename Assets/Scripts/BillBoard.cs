using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class BillBoard : NetworkBehaviour
{
    [SerializeField] private Transform cam;

    private void Awake()
    {
        cam = NetworkManager.SpawnManager.SpawnedObjects[NetworkManager.Singleton.LocalClient.PlayerObject.NetworkObjectId].transform.Find("CameraHolder/Main Camera").transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position - cam.forward);
    }
}

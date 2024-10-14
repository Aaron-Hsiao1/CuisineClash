using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpectatorCamera : NetworkBehaviour
{
	private Vector3 targetPosition;
	private Quaternion targetRotation;

	[SerializeField] private ulong targetClientId;
	[SerializeField] private float smoothSpeed = 0.01f;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		UpdatePosition();
	}

	public void SetCameraPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		targetPosition = position;
		targetRotation = rotation;
	}

	private void UpdatePosition()
	{
		GetClientPositionServerRpc(targetClientId, NetworkManager.Singleton.LocalClientId);
	}

	public void SetTargetClientId(ulong targetClientId)
	{
		this.targetClientId = targetClientId;
	}

	[ServerRpc(RequireOwnership = false)]
	private void GetClientPositionServerRpc(ulong targetClientId, ulong senderClientId)
	{
		GetClientPosition(targetClientId, senderClientId);
	}

	private void GetClientPosition(ulong targetClientId, ulong senderClientId)
	{
		foreach (var client in NetworkManager.Singleton.ConnectedClients)
		{
			if (client.Key == targetClientId)
			{
				Camera targetCamera = client.Value.PlayerObject.gameObject.transform.Find("CameraHolder").Find("Main Camera").GetComponent<Camera>(); ;
				RecieveClientPositionClientRpc(targetCamera.transform.position, targetCamera.transform.rotation, senderClientId);
			}
		}

		Debug.LogError("No player object from client id found");

	}

	[ClientRpc]
	private void RecieveClientPositionClientRpc(Vector3 position, Quaternion rotation, ulong updatedClient)
	{
		if (NetworkManager.Singleton.LocalClientId == updatedClient)
		{
			targetPosition = position;
			targetRotation = rotation;
		}

		SmoothCameraMovement();
	}

	private void SmoothCameraMovement()
	{
		// Update the camera's position and rotation smoothly
		transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
	}

}

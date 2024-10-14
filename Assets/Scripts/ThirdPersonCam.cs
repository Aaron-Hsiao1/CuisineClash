using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Timeline;

public class ThirdPersonCam : MonoBehaviour
{
	[SerializeField] private Transform orientation;
	[SerializeField] private Transform player;
	[SerializeField] private Transform playerObj;
	//[SerializeField] private Rigidbody rb;
	[SerializeField] private CinemachineFreeLook fL = null;

	[SerializeField] private float rotationSpeed;

	[SerializeField] private Transform combatLookAt;

	[SerializeField] private CameraStyle currentStyle;
	public enum CameraStyle
	{
		Basic,
		Combat
	}

    /*public override void OnNetworkSpawn()
	{
		/*if (IsOwner)
		{
			fL.Priority = 1;
		}
		else
		{
			fL.Priority = 0;
		}
		if (IsOwner && currentStyle == CameraStyle.Combat)
		{
			Debug.Log("fl used");
			Transform cameraTarget = combatLookAt;
			Transform cameraFollow = transform;
			if (fL = null)
			{
				Debug.Log("null");
				fL = FindObjectOfType<CinemachineFreeLook>();
			}
			if (fL != null)
			{
				Debug.Log("Not null");
				fL.Follow = cameraFollow;
				fL.LookAt = cameraTarget;
			}
		}
	}*/

    // Update is called once per frame
    void Update()
	{
		/*if (!IsLocalPlayer)
		{
			return;
		}*/
		//rotate orientation
		Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
		orientation.forward = viewDir.normalized;

		if (currentStyle == CameraStyle.Basic)
		{
			float horizontalInput = Input.GetAxis("Horizontal");
			float verticalInput = Input.GetAxis("Vertical");
			Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

			if (inputDir != Vector3.zero)
				playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
		}
		else if (currentStyle == CameraStyle.Combat)
		{
			Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
			orientation.forward = dirToCombatLookAt.normalized;

			playerObj.forward = dirToCombatLookAt.normalized;
		}
	}
}

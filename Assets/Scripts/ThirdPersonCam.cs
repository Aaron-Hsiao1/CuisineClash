using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Timeline;

public class ThirdPersonCam : NetworkBehaviour
{
	public Transform orientation;
	public Transform player;
	public Transform playerObj;
	public Rigidbody rb;
	[SerializeField] private CinemachineFreeLook fL = null;

	public float rotationSpeed;
	// Start is called before the first frame update

	public Transform combatLooktAt;

	public CameraStyle currentStyle;
	public enum CameraStyle
	{
		Basic,
		Combat
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked; //locks cursor
	}

	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			fL.Priority = 1;
		}
		else
		{
			fL.Priority = 0;
		}
		if (IsOwner)
		{
			Transform cameraTarget = transform;
			if (fL = null)
			{
				fL = GameObject.FindObjectOfType<CinemachineFreeLook>();
			}
			if (fL != null)
			{
				Debug.Log("Not null");
				fL.Follow = cameraTarget;
				fL.LookAt = cameraTarget;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsLocalPlayer)
		{
			return;
		}
		//rotate orientation
		Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
		orientation.forward = viewDir.normalized;


		if (currentStyle == CameraStyle.Basic)
		{
			//rotate player object

			float horizontalInput = Input.GetAxis("Horizontal");
			float verticalInput = Input.GetAxis("Vertical");
			Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
		}
		else if (currentStyle == CameraStyle.Combat)
		{
			Vector3 dirToCombatLookAt = combatLooktAt.position - new Vector3(transform.position.x, combatLooktAt.position.y, transform.position.z);
			orientation.forward = dirToCombatLookAt.normalized;

			playerObj.forward = dirToCombatLookAt.normalized;
		}



		//if (inputDir != Vector3.zero)
		//{
		//playerObj.forward = orientation.position;//Vector3.Slerp(playerObj.forward, dirToCombatLookAt.normalized, Time.deltaTime * rotationSpeed);
		//}
	}
}

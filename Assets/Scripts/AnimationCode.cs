using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class AnimationCode : NetworkBehaviour
{
	// Start is called before the first frame update
	private Animator mAnimator;
	public GameObject otherObject;
	private bool grounded;
	public LayerMask Ground;
	public float playerHeight;
	public KeyCode jumpKey = KeyCode.Space;
	public HotPotatoExplosion HPT;

	[SerializeField] private PlayerMovement playerMovement;

	void Awake()
	{
		mAnimator = otherObject.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsLocalPlayer)
		{
			return;
		}
		grounded = playerMovement.IsGrounded();
		Vector3 movementDirection = playerMovement.GetMoveDirection();

		if (mAnimator != null)
		{
			if (movementDirection != Vector3.zero)
			{
				mAnimator.SetBool("IsMoving", true);
			}
			else
			{
				mAnimator.SetBool("IsMoving", false);
			}
			if (grounded)
			{
				if (Input.GetKey(jumpKey))
				{
					mAnimator.SetBool("IsInAir", true);
				}
				else
				{
					mAnimator.SetBool("IsInAir", false);
				}
			}
			if (SceneManager.GetActiveScene().name == "HotPotato")
			{
				if (HPT.HasHotPotato())
				{
					mAnimator.SetBool("hasPotato", true);
					//Debug.Log("HAS HOT POTATO");
				}
				else
				{
					mAnimator.SetBool("hasPotato", false);
					//Debug.Log("DOES NOT HAVE HOT POTATO");
				}
			}

		}
	}
}

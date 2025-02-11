using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class AnimationCode : NetworkBehaviour
{
	// Start is called before the first frame update
	private Animator mAnimator;
	public GameObject otherObject;
	private bool grounded;
	public LayerMask Ground;
	public float playerHeight;
	public KeyCode jumpKey = KeyCode.Space;

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
            if (Input.GetKeyDown(KeyCode.B))
            {
                mAnimator.SetTrigger("TrHammer");
            }

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
		}
	}
}

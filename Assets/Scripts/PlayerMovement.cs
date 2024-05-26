using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed;

	public float groundDrag;

	public float playerHeight;
	public LayerMask Ground;
	bool grounded;

	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	bool canJump;

	public Transform orientation;

	float horizontalInput;
	float verticalInput;

	Vector3 moveDirection;

	Rigidbody rb;

	public KeyCode jumpKey = KeyCode.Space;
	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		canJump = true;
	}

	void Update()
	{
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

		myInput();
		speedControl();

		if (grounded)
		{
			rb.drag = groundDrag;
		}
		else
		{
			rb.drag = 0;
		}
		Debug.Log("Grounded " + grounded);
		Debug.Log("can Jump " + canJump);
	}

	private void FixedUpdate()
	{
		movePlayer();
	}

	private void myInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		if (Input.GetKey(jumpKey) && canJump && grounded)
		{
			canJump = false;
			Jump();
			Invoke(nameof(resetJump), jumpCooldown);
		}
	}

	private void movePlayer()
	{
		//calculate movement direction
		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
		rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

		if (grounded)
		{
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
		}
		else if (!grounded)
		{
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
		}
	}

	private void speedControl()
	{
		Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		//limits velocity
		if (flatVel.magnitude > moveSpeed)
		{
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
		}
	}

	private void Jump()
	{
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
		//rb.AddForce(Vector3.down * 200f, ForceMode.Force);
	}

	private void resetJump()
	{
		canJump = true;
	}
}

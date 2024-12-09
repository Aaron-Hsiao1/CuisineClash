using System.Collections;
using UnityEngine;

public class DashMechanic : MonoBehaviour
{
	[SerializeField] private PlayerMovement moveScript;
	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashTime;
	[SerializeField] private float cooldownTime = 5f;
	private float lastUsedTime;
	public GameObject dashParticles;
	private GameObject particles;



	private void Start()
	{
		moveScript = GetComponent<PlayerMovement>();
	}

	private void Update()
	{
		// Check if the Q key is pressed and the dash is not on cooldown
		if (Input.GetKeyDown(KeyCode.Q) && Time.time > lastUsedTime + cooldownTime)
		{
			StartCoroutine(Dash());
			particles = Instantiate(dashParticles, transform.position, Quaternion.identity);
			lastUsedTime = Time.time;
		}
		if (particles != null)
		{
			particles.transform.position = transform.position;
			particles.transform.rotation = transform.rotation;
		}
	}

	IEnumerator Dash()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		float startTime = Time.time;

		// Get dash direction from player movement input
		Vector3 dashDirection = moveScript.GetOrientation().forward * moveScript.GetVerticalInput() +
								moveScript.GetOrientation().right * moveScript.GetHorizontalInput();
		dashDirection = dashDirection.normalized;

		// Perform the dash
		while (Time.time < startTime + dashTime)
		{
			rb.velocity = dashDirection * dashSpeed;
			yield return null;
		}

		rb.velocity = Vector3.zero; // Stop movement after the dash
		//Destroy(particles);
	}
}

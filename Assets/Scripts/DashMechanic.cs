using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DashMechanic : MonoBehaviour
{
	[SerializeField] private PlayerMovement moveScript;
	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashTime;
	[SerializeField] private float cooldownTime = 5f;
	private float lastUsedTime;



	private void Start()
	{
		moveScript = GetComponent<PlayerMovement>();
	}

	private void Update()
	{
		// Check if the Q key is pressed and the dash is not on cooldown
		if (Input.GetKeyDown(KeyCode.Q) && Time.time > lastUsedTime + cooldownTime)
		{
			Dash();
			lastUsedTime = Time.time;
		}
	}

	private void Dash()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		float startTime = Time.time;

		// Get dash direction from player movement input
		Vector3 dashDirection = moveScript.GetOrientation().forward * moveScript.GetVerticalInput() +
								moveScript.GetOrientation().right * moveScript.GetHorizontalInput();
		dashDirection = dashDirection.normalized;

		StartCoroutine(LaunchPlayer(dashDirection * 10f, rb));	
	}


    private IEnumerator LaunchPlayer(Vector3 totalForce, Rigidbody rb)
    {
        rb.gameObject.GetComponentInParent<PlayerMovement>().SetLaunching(true);
		rb.velocity = new Vector3(0,0,0);
        rb.AddForce(totalForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.25f);
        rb.gameObject.GetComponentInParent<PlayerMovement>().SetLaunching(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
	public GameObject player;
	private Vector3 offset = new Vector3(0, 2.5f, 0);
	public GameObject orientation;

	private GameObject instantiatedObject;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			GameObject hitObject = hit.collider.gameObject;
			Debug.Log("raycast hit");
			//Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2.0f);
			if (hitObject.CompareTag("CanPickUp"))
			{
				instantiatedObject = Instantiate(hitObject, player.transform.position + offset, Quaternion.identity);
				Destroy(hitObject);
			}
		}
		else
		{
			Debug.Log("Raycast not hit");
		}

		if (instantiatedObject != null)
		{
			instantiatedObject.transform.position = transform.position + offset;
			instantiatedObject.transform.rotation = orientation.transform.rotation;
		}
	}
}

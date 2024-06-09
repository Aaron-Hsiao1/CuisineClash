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
	public GameObject orientation;
	public IngredientListSO ingredientList;

	public PlayerSandwich ps;

	public float placeDelay = 0.1f;

	private GameObject instantiatedObject;
	private Vector3 offset = new Vector3(0, 2.5f, 0);
	private Vector3 placeOffset = new Vector3(1.5f, 0, 0);
	public bool carrying;
	public bool canPlace;
	public bool canPickUp;
	public bool inPlaceZone;
	// Start is called before the first frame update
	void Start()
	{
		carrying = false;
		inPlaceZone = false;
		canPlace = true;
		canPickUp = true;
	}

	// Update is called once per frame
	void Update()
	{


		if (Input.GetKeyDown(KeyCode.E))
		{
			if (carrying && canPlace)
			{
				PlaceObject();
				StartCoroutine(PlaceDelayCoroutine());
			}
			else
			{
				PickUp();
			}
		}

		if (instantiatedObject != null)
		{
			instantiatedObject.transform.position = transform.position + offset;
			instantiatedObject.transform.rotation = orientation.transform.rotation;
		}


	}

	public void PickUp()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 25))
		{
			GameObject hitObject = hit.collider.gameObject;
			//Debug.Log("raycast hit");
			//Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2.0f);
			if (hitObject.CompareTag("CanPickUp") && Input.GetKeyDown(KeyCode.E) && !carrying && canPickUp)
			{
				instantiatedObject = Instantiate(hitObject, player.transform.position + offset, Quaternion.identity);
				instantiatedObject.name = instantiatedObject.name.Replace("(Clone)", "").Trim();
				Destroy(hitObject);
				carrying = true;
				canPickUp = false;
			}
		}
		else
		{
			//Debug.Log("Raycast not hit");
		}
	}

	public void PlaceObject()
	{
		Instantiate(instantiatedObject, player.transform.position + placeOffset, Quaternion.identity);
		instantiatedObject.SetActive(false);
		if (inPlaceZone)
		{
			foreach (IngredientSO ingredient in ingredientList.ingredientListSO)
			{
				if (ingredient.ingredientName == instantiatedObject.name)
				{
					ps.playerSandwich.Insert(0, ingredient.prefab);
				}
			}

		}
		carrying = false;
		canPickUp = true;
	}

	IEnumerator PlaceDelayCoroutine()
	{
		canPlace = false;
		yield return new WaitForSeconds(placeDelay);
		canPlace = true;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Placing Zone"))
		{
			inPlaceZone = true;
		}
	}
	public void OnTriggerExit(Collider other)
	{
		inPlaceZone = false;
	}

}

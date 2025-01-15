using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AttachScript : NetworkBehaviour
{
	public GameObject childObject; // Assign the child object (weapon)
	public GameObject parentObject; // Assign the parent object (character's hand)
	public HotPotatoExplosion HPT;

	private bool potatocreated = false;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (HPT.HasHotPotato() && !potatocreated)
		{
			if (childObject != null && parentObject != null)
			{
				InstantiateNewHotPotato();

			}
			potatocreated = true;
		}
		if (!HPT.HasHotPotato() && potatocreated)
		{
			Destroy(childObject);
		}
	}

	public void InstantiateNewHotPotato()
	{
		childObject = Instantiate(childObject, transform.position, Quaternion.identity);
		childObject.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
		childObject.transform.SetParent(parentObject.transform, true);
		childObject.transform.localPosition = new Vector3(0.0204f, 0.0111f, 0.0106f);
		childObject.transform.localRotation = Quaternion.Euler(-97.53f, 125.794f, -292.38f);
	}

	[ClientRpc]
	private void InstantiateNewHotPotatoClientRpc()
	{

	}
}


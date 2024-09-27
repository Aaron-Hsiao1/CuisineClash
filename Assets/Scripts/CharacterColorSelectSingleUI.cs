using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
	[SerializeField] private int colorId;
	[SerializeField] private Image image;
	[SerializeField] private GameObject selectedGameObject;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			CuisineClashMultiplayer.Instance.ChangePlayerColor(colorId);
		});
	}

	private void Start()
	{
		CuisineClashMultiplayer.Instance.OnPlayerDataNetworkListChanged += CuisineClashMultiplayer_OnPlayerDataNetworkListChanged;
		image.color = CuisineClashMultiplayer.Instance.getPlayerColor(colorId);
		UpdateIsSelected();
	}

	private void CuisineClashMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
	{
		UpdateIsSelected();
	}

	private void UpdateIsSelected()
	{
		if (CuisineClashMultiplayer.Instance.GetPlayerData().colorId == colorId)
		{
			selectedGameObject.SetActive(true);
		}
		else
		{
			selectedGameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		CuisineClashMultiplayer.Instance.OnPlayerDataNetworkListChanged -= CuisineClashMultiplayer_OnPlayerDataNetworkListChanged;
	}
}
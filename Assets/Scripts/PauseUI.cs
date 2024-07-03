using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button leaveGameButton;

	[SerializeField] private GameObject settingsUI;

	private void Awake()
	{
		settingsButton.onClick.AddListener(() =>
		{
			settingsUI.gameObject.SetActive(true);
		});
		leaveGameButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scene.MainMenu);
		});
	}
}

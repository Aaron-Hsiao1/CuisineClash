using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button leaveGameButton;

	[SerializeField] private Canvas settingsUI;

	private void Awake()
	{
		settingsButton.onClick.AddListener(() =>
		{
			settingsUI.gameObject.SetActive(true);
		});
		leaveGameButton.onClick.AddListener(() =>
		{
			Loader.Load(Loader.Scene.MainMenu);
		});
	}
}

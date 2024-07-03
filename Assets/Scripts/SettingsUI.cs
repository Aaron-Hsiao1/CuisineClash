using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
	[SerializeField] private Button exitButton;
	[SerializeField] private GameObject settingsMenu;

	private void Awake()
	{
		exitButton.onClick.AddListener(() =>
		{
			settingsMenu.SetActive(false);
		});
	}
}

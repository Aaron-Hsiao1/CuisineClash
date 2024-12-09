using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : NetworkBehaviour
{
    [SerializeField] private Button returnToMainMenu;
    [SerializeField] private Camera secondaryCamera;
    [SerializeField] private GameObject disconnectUI;

    private void Awake()
    {
        returnToMainMenu.onClick.AddListener(() =>
        {
            ShowSecondaryCamera();
                
        });
    }
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
    }
    private void OnDisable()
    {
        Debug.Log("On Disbale");
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectedCallback;
    }


    private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId || clientId == NetworkManager.Singleton.LocalClientId)
        {
            ShowSecondaryCamera();
            
            //Show();
        }
    }

    private void ShowSecondaryCamera()
    {
        ShowSecondaryCameraClientRpc();
        Debug.Log("Client rpc called");
    }

    [ClientRpc()]
    private void ShowSecondaryCameraClientRpc()
    {
        Debug.Log("Secondary camera being set to active...");
        secondaryCamera.gameObject.SetActive(true);
        secondaryCamera.tag = "MainCamera";
        Cursor.lockState = CursorLockMode.None;
        disconnectUI.SetActive(true);
        //Show();
    }

    private void Show()
    {
        Cursor.lockState = CursorLockMode.None;
        disconnectUI.SetActive(true);
    }
    private void Hide()
    {
        disconnectUI.SetActive(false);
    }
}

/*using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : NetworkBehaviour
{
	[SerializeField] private Button returnToMainMenu;
	[SerializeField] private Camera secondaryCamera;
	[SerializeField] private GameObject disconnectUI;

	private void Awake()
	{
		Hide();
		returnToMainMenu.onClick.AddListener(() =>
		{
			Hide();
			Loader.Load(Loader.Scene.MainMenu);
		});
	}

	private void Start()
	{
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
	}


	private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
	{
		Debug.Log("Some dced");
		if (clientId == NetworkManager.ServerClientId || clientId == NetworkManager.Singleton.LocalClientId)
		{
			Debug.Log("It was the host");
			ShowSecondaryCamera();

        }
	}

	private void ShowSecondaryCamera()
	{
        Debug.Log("Secondary camera being set to active...");
        secondaryCamera.gameObject.SetActive(true);
        //secondaryCamera.tag = "MainCamera";
        Cursor.lockState = CursorLockMode.None;
        disconnectUI.SetActive(true);

        Debug.Log("Show secondary camera non client rpc called");
		//ShowSecondaryCameraClientRpc();

	}
	
	[ClientRpc()]
	private void ShowSecondaryCameraClientRpc()
	{
		Debug.Log("Secondary camera being set to active...");
		secondaryCamera.gameObject.SetActive(true);
        secondaryCamera.tag = "MainCamera";
        Cursor.lockState = CursorLockMode.None;
        disconnectUI.SetActive(true);
        //Show();
    }

	private void Show()
	{
		Cursor.lockState = CursorLockMode.None;
		disconnectUI.SetActive(true);
	}
	private void Hide()
	{
		disconnectUI.SetActive(false);
	}
}*/

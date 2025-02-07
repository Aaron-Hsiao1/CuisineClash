using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
	[SerializeField] private Button closeButton;
	[SerializeField] private Button createPublicButton;
	[SerializeField] private Button createPrivateButton;
	[SerializeField] private TMP_InputField lobbyNameInputField;

	private void Awake()
	{
		createPublicButton.onClick.AddListener(() =>
		{
			LoadNextLevel();
			CuisineClashLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
		});
		createPrivateButton.onClick.AddListener(() =>
		{
			CuisineClashLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
		});
		closeButton.onClick.AddListener(() =>
		{
// Hide();
		});
	}

	private void Start()
	{
		Hide();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}

    public Animator transition;

    public float transitionTime = 1f;
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}

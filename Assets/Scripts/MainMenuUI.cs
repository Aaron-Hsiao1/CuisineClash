using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Transition")]
    public Animator transition;
    public float transitionTime = 1f;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            LoadNextLevel();
            Loader.Load(Loader.Scene.ConnectionMenu);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
    
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
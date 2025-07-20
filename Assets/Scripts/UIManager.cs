using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject YouWinPanel;
    // Start is called before the first frame update
    public Button StartButton;
    public Button RestartButton;
    public Button MainMenuButton;
    public static bool isRestarting = false;
    public bool isGamestarted = false;
    void Start()
    {
        StartButton.onClick.AddListener(startGame);
        RestartButton.onClick.AddListener(RestartGame);
        MainMenuButton.onClick.AddListener(GotoMainMenu);
        if (!isRestarting)
        {
            MainMenuPanel.SetActive(true);
            Time.timeScale = 0f;

        }
        else
        {
            startGame();
        }
    }

    // Update is called once per frame

    public void startGame()
    {
        StartCoroutine(setBool());
        MainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    private IEnumerator setBool()
    {
        yield return new WaitForSeconds(2f);
        isGamestarted = true;

    }
    public void RestartGame()
    {
        isRestarting = true;
        SceneManager.LoadScene(0);

    }
    public void GotoMainMenu()
    {
        isRestarting = false;
        SceneManager.LoadScene(0);

    }
    public void GameOver()
    {
        YouWinPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        Time.timeScale = 0f;

    }
}

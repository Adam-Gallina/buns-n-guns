using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenus : MonoBehaviour
{
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject winMenu;

    private void Awake()
    {
        SetPauseMenuVisibility(false);
        SetDeathMenuVisibility(false);
        SetWinMenuVisibility(false);
        UpdateBackgroundVisibility();
    }

    public void HideMenus()
    {
        SetPauseMenuVisibility(false);
        SetDeathMenuVisibility(false);
        SetWinMenuVisibility(false);
    }

    private void UpdateBackgroundVisibility()
    {
        menuBackground.SetActive(pauseMenu.activeSelf ||
                                 deathMenu.activeSelf ||
                                 winMenu.activeSelf);
    }

    public void SetPauseMenuVisibility(bool showMenu)
    {
        pauseMenu.SetActive(showMenu);
        UpdateBackgroundVisibility();
    }

    public void SetDeathMenuVisibility(bool showMenu)
    {
        deathMenu.SetActive(showMenu);
        UpdateBackgroundVisibility();
    }

    public void SetDeathMenuText(string text)
    {
        deathMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = text;
    }

    public void SetWinMenuVisibility(bool showMenu)
    {
        winMenu.SetActive(showMenu);
        UpdateBackgroundVisibility();
    }

    public void SetWinMenuText(string text, float time)
    {
        int min = (int)time / 60;
        float sec = time - min * 60;
        string timeText = min.ToString() + ":" + System.Math.Round(sec, 2).ToString();

        winMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = text;
        winMenu.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Time: " + timeText;
    }

    public void ResumeButton()
    {
        LevelController.instance.SetPause(false);
    }
    public void RestartButton()
    {
        GameController.instance.LoadLevel(GameController.instance.currLevel);
    }

    public void QuitButton()
    {
        GameController.instance.ReturnToMenu();
    }
}

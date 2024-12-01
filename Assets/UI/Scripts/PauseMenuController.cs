using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI pauseMenuTitle;

    public void Pause()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void Save()
    {
        GameManager.Instance.SaveGame();
        pauseMenuTitle.text = "Jogo Salvo";
        pauseMenuTitle.color = Color.green;
    }

    public void SaveAndExit()
    {
        Time.timeScale = 1;
        Save();
        GameManager.Instance.BackToMainMenu();
    }
}

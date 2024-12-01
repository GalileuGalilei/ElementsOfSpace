using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private Image loadGameButton;

    private void Start()
    {
        if (!GameManager.Instance.CheckPlayerData())
        {
            loadGameButton.color = Color.gray;
            loadGameButton.GetComponent<Button>().interactable = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

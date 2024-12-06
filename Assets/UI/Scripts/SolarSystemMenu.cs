using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemMenu : MonoBehaviour
{
    public void ExitSolarSystemMenu()
    {
        GameManager.Instance.ShowSolarSystemMenu(false);
    }

    public void InitializeEarthScene()
    {
        GameManager.Instance.InitializeEarthScene();
    }
}

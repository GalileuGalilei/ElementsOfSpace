using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetUIController : MonoBehaviour
{
    [SerializeField]
    private string planetName;
    [SerializeField]
    private GameObject planetDescription;
    private Animator animator;
    private RectTransform SolarSystem;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(planetName);
        SolarSystem = transform.parent.GetComponent<RectTransform>();
    }

    public void EnterPlanet()
    {
        planetDescription.SetActive(false);
        //animator.Play("Enter");
        GameManager.Instance.LoadNewGame(planetName);
    }

    public void ZoomIn()
    {
        float zoomIn = 1.5f / transform.localScale.z;

        if (planetDescription.activeSelf)
        {
            planetDescription.SetActive(false);
            SolarSystem.localScale = Vector3.one;
            SolarSystem.position = Vector3.zero;
        }
        else
        {
            planetDescription.SetActive(true);
            Vector3 position = GetComponent<RectTransform>().localPosition * -zoomIn;
            SolarSystem.localPosition = new Vector3(position.x, position.y, 0);
            SolarSystem.localScale = new Vector3(zoomIn, zoomIn, 1);
        }
    }
}

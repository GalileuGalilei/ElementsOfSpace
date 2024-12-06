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

    private Vector3 originalPos = Vector3.zero;
    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play(planetName);
        SolarSystem = transform.parent.GetComponent<RectTransform>();
    }

    public void EnterPlanet()
    {
        planetDescription.SetActive(false);
        //animator.Play("Enter");
        GameManager.Instance.LoadNewPlanet(planetName);
    }

    public void ZoomIn()
    {
        float zoomIn = 3.0f / (transform.localScale.z * 0.85f);

        if (planetDescription.activeSelf)
        {
            planetDescription.SetActive(false);
            SolarSystem.localScale = originalScale;
            SolarSystem.position = originalPos;
        }
        else
        {
            originalPos = SolarSystem.position;
            originalScale = SolarSystem.localScale;

            planetDescription.SetActive(true);
            Vector3 position = GetComponent<RectTransform>().localPosition * -zoomIn;
            SolarSystem.localPosition = new Vector3(position.x, position.y, 0);
            SolarSystem.localScale = new Vector3(zoomIn, zoomIn, 1);
        }
    }
}

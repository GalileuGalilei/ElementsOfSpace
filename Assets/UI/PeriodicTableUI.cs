using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PeriodicTableUI : MonoBehaviour
{
    [SerializeField]
    private GameObject elementUIPrefab;
    private int elementHeight;
    private int elementWidth;
    private HashSet<string> foundElements = new();

    private RectTransform rectTransform;
    public float fadeLevel = 0.5f;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        //tabela periodica normalmente eh 18x9
        elementHeight = (int)rectTransform.rect.height / 9;
        elementWidth = (int)rectTransform.rect.width / 18;

        BuildPeriodicTableUI();
    }
    /*
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            FoundElement("Fe", 2, 1.5f, 2, 1.5f);
        }
    }
    */
    public void FoundElement(string symbol, float foundAnimationTime, float fadeInTime, float keepOnScreenTime, float fadeOutTime)
    {
        foundElements.Add(symbol);
        ElementUI elementUI = transform.Find(symbol).GetComponent<ElementUI>();
        
        StartCoroutine(elementUI.ElementFoundAnimation(foundAnimationTime));
        StartCoroutine(FadeInAndOutTable(fadeInTime, keepOnScreenTime, fadeOutTime));
    }

    private void BuildPeriodicTableUI()
    {
        PeriodicTable table = PeriodicTable.Instance;

        foreach (Element element in table.elements)
        {
            GameObject elementUI = Instantiate(elementUIPrefab, transform);

            RectTransform elementUIRect = elementUI.GetComponent<RectTransform>();
            elementUIRect.sizeDelta = new Vector2(elementWidth, elementHeight);
            elementUIRect.anchoredPosition = new Vector2(element.xpos * elementWidth, -element.ypos * elementHeight);
            elementUIRect.anchorMin = new Vector2(0, 1);
            elementUIRect.anchorMax = new Vector2(0, 1);

            ElementUI elementUIComponent = elementUI.GetComponent<ElementUI>();
            elementUIComponent.SetColor(GetCategoryColor(element.category));
            elementUIComponent.SetFadeLevel(0.0f);
            elementUIComponent.SetSymbol(element.symbol);
            elementUI.transform.name = element.symbol;
        }
    }

    private Color GetCategoryColor(string category)
    {
        switch (category)
        {
            case "alkali metal":
                return new Color(0.8f, 0.8f, 0.8f);
            case "alkaline earth metal":
                return new Color(0.4f, 0.6f, 0.8f);
            case "transition metal":
                return new Color(0.8f, 0.2f, 0.2f);
            case "post-transition metal":
                return new Color(0.8f, 0.5f, 0.5f);
            case "metalloid":
                return new Color(0.5f, 0.5f, 0.8f);
            case "polyatomic nonmetal":
                return new Color(0.1f, 0.3f, 0.8f);
            case "diatomic nonmetal":
                return new Color(0.8f, 0.5f, 0.1f);
            case "noble gas":
                return new Color(0.8f, 0.8f, 0.1f);
            case "lanthanide":
                return new Color(0.8f, 0.1f, 0.1f);
            case "actinide":
                return new Color(0.4f, 0.3f, 0.5f);
            default:
                Debug.Log("category color did not found");
                return new Color(0.7f, 0.7f, 0.5f);
        }
    }

    #region animations
    private IEnumerator FadeInAndOutTable(float FadeInTime, float keepOnScreenTime, float fadeOutTime)
    {
        float fadeLevel = 0;
        while (fadeLevel < this.fadeLevel)
        {
            fadeLevel += Time.deltaTime * FadeInTime;
            foreach (Transform child in transform)
            {
                ElementUI elementUI = child.GetComponent<ElementUI>();

                if (foundElements.Contains(elementUI.Symbol))
                {
                    elementUI.SetFadeLevel(1.0f);
                    continue;
                }

                elementUI.SetFadeLevel(fadeLevel);
            }
            yield return null;
        }

        yield return new WaitForSeconds(keepOnScreenTime);
        StartCoroutine(FadeOutTable(fadeOutTime));
    }

    private IEnumerator FadeOutTable(float speed)
    {
        float fadeLevel = this.fadeLevel;
        while (fadeLevel > 0)
        {
            fadeLevel -= Time.deltaTime * speed;
            foreach (Transform child in transform)
            {
                ElementUI elementUI = child.GetComponent<ElementUI>();
                elementUI.SetFadeLevel(fadeLevel);
            }
            yield return null;
        }
    }

    #endregion animations
}

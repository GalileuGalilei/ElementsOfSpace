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

    private RectTransform rectTransform;

    public float fadeLevel = 0.5f;

    //lista de animacoes para serem executadas em sequencia
    private List<IEnumerator> animations = new List<IEnumerator>();


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        //tabela periodica normalmente eh 18x9
        elementHeight = (int)rectTransform.rect.height / 9;
        elementWidth = (int)rectTransform.rect.width / 18;

        BuildPeriodicTableUI();
    }

    public void FoundElement(string symbol)
    {
        //fade in the table and after coroutine ends, fade out the table
        


        ElementUI elementUI = transform.Find(symbol).GetComponent<ElementUI>();
        elementUI.StartCoroutine(elementUI.ElementFoundAnimation());
    }

    private IEnumerator FadeInTable()
    {
        float fadeLevel = 0;
        while (fadeLevel < this.fadeLevel)
        {
            fadeLevel += Time.deltaTime;
            foreach (Transform child in transform)
            {
                ElementUI elementUI = child.GetComponent<ElementUI>();
                elementUI.SetFadeLevel(fadeLevel);
            }
            yield return null;
        }
    }

    private IEnumerator FadeOutTable()
    {
        float fadeLevel = this.fadeLevel;
        while (fadeLevel > 0)
        {
            fadeLevel -= Time.deltaTime;
            foreach (Transform child in transform)
            {
                ElementUI elementUI = child.GetComponent<ElementUI>();
                elementUI.SetFadeLevel(fadeLevel);
            }
            yield return null;
        }
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
            elementUIComponent.SetFadeLevel(fadeLevel);
            elementUIComponent.SetColor(GetCategoryColor(element.category));
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
}

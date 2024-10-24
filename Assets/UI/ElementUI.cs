using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI symbol;
    [SerializeField]
    private UnityEngine.UI.Image background;    

    public void SetFadeLevel(float fadeLevel)
    {
        Color color = background.color;
        color.a = fadeLevel;
        background.color = color;
    }

    public void SetSymbol(string symbol)
    {
        this.symbol.text = symbol;
    }

    public void SetColor(Color color)
    {
        this.background.color = color;
    }

    public IEnumerator ElementFoundAnimation()
    {
        float fadeLevel = 1;
        while (fadeLevel > 0)
        {
            fadeLevel -= Time.deltaTime;
            SetFadeLevel(fadeLevel);
            yield return null;
        }
    }
}

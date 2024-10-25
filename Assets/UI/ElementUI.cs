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
    public string Symbol => symbol.text;

    public void SetFadeLevel(float fadeLevel)
    {
        Color color = background.color;
        color.a = fadeLevel;
        background.color = color;

        symbol.color = new Color(1, 1, 1, fadeLevel);
    }

    public void SetSymbol(string symbol)
    {
        this.symbol.text = symbol;
    }

    public void SetColor(Color color)
    {
        this.background.color = color;
    }

    public IEnumerator ElementFoundAnimation(float foundAnimationTime)
    {
        const float initialSizeMultiplier = 5.0f;
        const float finalSizeMultiplier = 1.0f;
        float speed = initialSizeMultiplier / foundAnimationTime;

        float sizeMultiplier = initialSizeMultiplier;
        while (sizeMultiplier > finalSizeMultiplier)
        {
            sizeMultiplier -= Time.deltaTime * speed;
            transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, 1);
            yield return null;
        }
    }
}

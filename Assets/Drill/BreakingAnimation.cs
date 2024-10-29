using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakingAnimation : MonoBehaviour
{
    public UnityEvent onAnimationFinish;

    //called by the animation event
    public void OnAnimationFinishallback()
    {
        onAnimationFinish?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        onAnimationFinish?.RemoveAllListeners();
    }
}

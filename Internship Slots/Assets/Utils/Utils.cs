using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static IEnumerator CounterCoroutine(Text counterText, int startValue, int endValue)
    {
        for (var i = startValue; i <= endValue; i++)
        {
            counterText.text = i.ToString();
            if (endValue - startValue > 50f)
            {
                yield return new WaitForSeconds(0.005f);
            } 
            else
            {
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}

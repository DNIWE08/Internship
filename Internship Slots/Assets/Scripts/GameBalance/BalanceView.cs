using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceView : MonoBehaviour
{
    [SerializeField] private Counter prizeCounter;
    [SerializeField] private Counter freeSpinCounter;
    [SerializeField] CanvasGroup fsPnlCG;

    public void ChangeValue(int nextValue)
    {
        StartCoroutine(CounterCoroutine(prizeCounter.CounterText, 0, nextValue));
    }
    
    public void ChangeFreeSpinValue(int lastValue, int nextValue)
    {
        StartCoroutine(CounterCoroutine(prizeCounter.CounterText, lastValue, nextValue));
    }

    public void ResetPrizeCounter(int prize)
    {
        prizeCounter.CounterText.text = prize.ToString();
    }

    public void UpdateFreeSpinCount(int count)
    {
        freeSpinCounter.CounterText.text = count.ToString();
    }

    public void StopCoroutine()
    {
        StopAllCoroutines();
    }

    public IEnumerator CounterCoroutine(Text counterText, int startValue, int endValue)
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

    public void ToggleFreeSpinPanel(bool isFreeSpin)
    {
        if (isFreeSpin)
        {
            fsPnlCG.alpha = 1;
        }
        else
        {
            fsPnlCG.alpha = 0;
        }
    }
}

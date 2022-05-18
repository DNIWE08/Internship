using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceView : MonoBehaviour
{
    [SerializeField] private Counter prizeCounter;
    [SerializeField] private Counter freeSpinCounter;
    [SerializeField] CanvasGroup fsPnlCG;
    [SerializeField] private AudioController audioController;

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
            var delay = 0.02f;
            counterText.text = i.ToString();
            if (endValue - startValue > 50f)
            {
                delay = 0.005f;
            }
            audioController.PlayAudio(AudioType.SFX_Counter);
            yield return new WaitForSeconds(delay);
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

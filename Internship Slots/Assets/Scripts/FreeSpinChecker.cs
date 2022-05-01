using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FreeSpinChecker : MonoBehaviour
{
    [SerializeField] Reel[] reels;
    [SerializeField] Text CounterText;
    
    [SerializeField] GameObject fsPnl;
    [SerializeField] Transform fsPopup;
    [SerializeField] Text fsPrize;
    [SerializeField] int bonusSpin = 10;
    private CanvasGroup fsPnlCG;
    private int freeSpinCount = 0;
    private float totalFreeSpinPrize = 0;
    internal bool isFreeSpin = false;

    internal int FreeSpinCount { get => freeSpinCount; set => freeSpinCount = value; }
    internal float TotalFreeSpinPrize { get => totalFreeSpinPrize; set => totalFreeSpinPrize = value; }

    public static event Action CheckFreeSpin;

    private void Start()
    {
        fsPopup.transform.localScale = Vector3.zero;
        fsPnlCG = fsPnl.GetComponent<CanvasGroup>();
        CheckFreeSpin += OnFreeSpin;
    }

    private bool ScatterOnReels()
    {
        if (reels[0].hasScatter && reels[1].hasScatter && reels[2].hasScatter)
        {
            return true;
        }
        return false;
    }

    public static void StartFreeSpin()
    {
        CheckFreeSpin?.Invoke();
    }

    private void OnFreeSpin()
    {
        if (ScatterOnReels())
        {
            if(freeSpinCount == 0)
            {
                totalFreeSpinPrize = 0;
            }
            isFreeSpin = true;
            fsPnlCG.alpha = 1;
            freeSpinCount += bonusSpin;
        }
        else if (freeSpinCount == 0)
        {
            isFreeSpin = false;
            fsPnlCG.alpha = 0;
        }
        CounterText.text = freeSpinCount.ToString();
    }

    internal void ShowFreeSpinPopup()
    {
        fsPrize.text = "0";
        fsPopup
            .DOScale(new Vector3(1,1,1), 0.3f)
            .OnComplete(() =>
            {
                fsPrize.GetComponent<Transform>()
                    .DOScale(1.2f, 0.3f)
                    .SetLoops(8, LoopType.Yoyo);
                StartCoroutine(CounterCorutine());
            });
    }

    public void ClosePopup()
    {
        fsPopup.DOScale(new Vector3(0, 0, 0), 0.3f);
    }

    private IEnumerator CounterCorutine()
    {
        for (var i = 0; i <= totalFreeSpinPrize; i++)
        {
            fsPrize.text = i.ToString();
            yield return new WaitForSeconds(0.005f);
        }
    }
}

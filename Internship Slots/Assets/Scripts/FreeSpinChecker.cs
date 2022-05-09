using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FreeSpinChecker : MonoBehaviour
{
    [SerializeField] Reel[] reels;
    [SerializeField] BalanceController balanceController;
    [SerializeField] ReelSpinner reelSpinner;
    
    [SerializeField] Transform fsPopup;
    [SerializeField] Text fsPrize;
    [SerializeField] int bonusSpin = 10;

    public static event Action CheckFreeSpin;

    private void Start()
    {
        fsPopup.transform.localScale = Vector3.zero;
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

    public static void StartCheckFreeSpin()
    {
        CheckFreeSpin?.Invoke();
    }

    private void OnFreeSpin()
    {
        if (ScatterOnReels())
        {
            if(balanceController.BalanceModel.FreeSpinCount == 0)
            {
                balanceController.StartFreeSpin();
            }
            balanceController.UpdateFreeSpin(bonusSpin);
        }
        balanceController.UpdateFreeSpinCount();
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
                StartCoroutine(Utils.CounterCoroutine(fsPrize, 0, balanceController.BalanceModel.TotalFreeSpinPrize));
            });
    }

    public void ClosePopup()
    {
        fsPopup.DOScale(new Vector3(0, 0, 0), 0.3f);
        reelSpinner.ReelsState = ReelStateEnum.Ready;
    }
}

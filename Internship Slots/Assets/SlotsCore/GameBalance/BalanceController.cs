using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceController : MonoBehaviour
{
    [SerializeField] private BalanceModel balanceModel;
    [SerializeField] private BalanceView balanceView;
    [SerializeField] private AudioController audioController;

    public BalanceModel BalanceModel { get => balanceModel; }
    public BalanceView BalanceView { get => balanceView; }

    public void GetSpinPrize(int prize)
    {
        balanceModel.CurrentPrize = prize;
    }

    public void ChangeBalance()
    {
        var currentPrize = balanceModel.CurrentPrize;

        if (currentPrize == 0) return;

        if(balanceModel.IsFreeSpin)
        {
            var lastFreeSpinPrize = balanceModel.TotalFreeSpinPrize;
            balanceModel.TotalFreeSpinPrize += currentPrize;
            balanceView.ChangeFreeSpinValue(lastFreeSpinPrize, balanceModel.TotalFreeSpinPrize);
        }
        else
        {
            balanceView.ChangeValue(currentPrize);
        }
    }

    public void UpdateFreeSpinCount()
    {
        balanceView.UpdateFreeSpinCount(balanceModel.FreeSpinCount);
    }

    public void PrepareCounter()
    {
        balanceModel.CurrentPrize = 0;
        if (balanceModel.IsFreeSpin)
        {
            balanceView.ResetPrizeCounter(balanceModel.TotalFreeSpinPrize);
        }
        else
        {
            balanceView.ResetPrizeCounter(balanceModel.CurrentPrize);
        }
    }

    public void StartFreeSpin()
    {
        balanceModel.TotalFreeSpinPrize = 0;
        balanceModel.IsFreeSpin = true;
        balanceView.ToggleFreeSpinPanel(balanceModel.IsFreeSpin);
        audioController.PlayAudio(AudioType.BG_02);
    }

    public void UpdateFreeSpin(int bonusSpin)
    {
        balanceModel.FreeSpinCount += bonusSpin;
        balanceModel.IsFreeSpin = true;
        balanceView.ToggleFreeSpinPanel(balanceModel.IsFreeSpin);
    }

    public void EndFreeSpin()
    {
        balanceModel.IsFreeSpin = false;
        balanceView.ToggleFreeSpinPanel(balanceModel.IsFreeSpin);
        audioController.PlayAudio(AudioType.BG_01);
    }

    public void StopCoroutine()
    {
        balanceView.StopCoroutine();
    }

    public void PrintData()
    {
        print(balanceModel.FreeSpinCount + " Free Spins");
        print(balanceModel.CurrentPrize + " Current prize");
        print(balanceModel.IsFreeSpin + " is Free Spins");
    }
}

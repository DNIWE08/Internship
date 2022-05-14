using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceModel : MonoBehaviour
{
    private int currentPrize = 0;
    private int totalFreeSpinPrize = 0;
    private int freeSpinCount = 0;
    private bool isFreeSpin = false;

    public int TotalFreeSpinPrize { get => totalFreeSpinPrize; set => totalFreeSpinPrize = value; }
    public int CurrentPrize { get => currentPrize; set => currentPrize = value; }
    public int FreeSpinCount { get => freeSpinCount; set => freeSpinCount = value; }
    public bool IsFreeSpin { get => isFreeSpin; set => isFreeSpin = value; }
}

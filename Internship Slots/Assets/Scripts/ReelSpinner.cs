using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ReelSpinner : MonoBehaviour
{
    [SerializeField] private Reel[] reels;
    [SerializeField] private int spinCount;
    [SerializeField] private int symbolsOnReel;
    [SerializeField] private float spinDuration = 1;
    [SerializeField] private float winLineDuration = 1.2f;

    [SerializeField] public Button startBtn;
    [SerializeField] public Button stopBtn;

    [SerializeField] private GameObject anticipationParticle;

    [SerializeField] private BalanceController balanceController;

    [SerializeField] private AudioController audioController;

    private float spinIteration;
    private float symbolHeight;

    private FreeSpinChecker freeSpinComponent;
    private bool isFinalFreeSpin = false;

    public static event Action OnReelsStart;

    private Dictionary<Transform, Reel> reelsDictionary;
    private ReelStateEnum reelsState = ReelStateEnum.Ready;
    internal ReelStateEnum ReelsState { get => reelsState; set => reelsState = value; }


    private void Start()
    {
        freeSpinComponent = gameObject.GetComponent<FreeSpinChecker>();

        symbolHeight = reels[1].ReelSymbols[1].GetComponent<RectTransform>().rect.height;

        spinIteration = -symbolHeight * reels[1].ReelSymbols.Length;

        reelsDictionary = new Dictionary<Transform, Reel>();

        for (var i = 0; i < reels.Length; i++)
        {
            var reelT = reels[i].transform;
            reelsDictionary.Add(reelT, reels[i]);
        }
        OnReelsStart += StartState;
        //WinLineChecker.OnForceSpinStart += StartState;
    }

    private void Update()
    {
        CheckButtonState();
    }

    public void StartSpin()
    {
        audioController.PlayLoopAudio(AudioType.SFX_ReelsScroll);
        //WinLineChecker.ForceSpinStart();
        //StartState();
        OnReelsStart?.Invoke();
        for (int i = 0; i < reels.Length; i++)
        {
            var currentReel = reels[i];
            currentReel.isFinalSpin = false;
            currentReel.ResetScatter();
            ReelStopped(currentReel, false);

            var reelT = currentReel.transform;
            reelT.DOLocalMoveY(spinIteration, 0.6f)
            .SetEase(Ease.InCubic)
            .SetDelay(i * 0.2f)
            .OnComplete(() =>
            {
                if (i == reels.Length)
                {
                    reelsState = ReelStateEnum.Spin;
                }
                MiddleSpin(reelT);
            });
        }
    }

    public void MiddleSpin(Transform reelT)
    {
        reelsState = ReelStateEnum.Spin;
        DOTween.Kill(reelT);
        var reelDistance = spinIteration * spinCount;
        reelT.DOLocalMoveY(reelDistance, spinDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => CorrectSpin(reelT));
    }

    public void ScrollStop(Transform reelT)
    {
        DOTween.Kill(reelT);
        reelsDictionary[reelT].isFinalSpin = true;
        var currentReelPosY = reelT.localPosition.y;
        var stoppingDistance = currentReelPosY - symbolHeight * symbolsOnReel;
        reelT.DOLocalMoveY(stoppingDistance, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                audioController.PlayAudio(AudioType.SFX_ReelsStop);
                if(reelsDictionary[reelT].hasScatter == true)
                {
                    audioController.PlayAudio(AudioType.SFX_Scatter);
                }
                PrepareReel(reelT);
                if (reelsDictionary[reelT].reelId == reels.Length)
                {
                    anticipationParticle.SetActive(false);
                    audioController.StopAudio(AudioType.SFX_Anticipation);
                    WinLineChecker.StartCheckAnimation();
                    FreeSpinChecker.StartCheckFreeSpin();
                    reelsState = ReelStateEnum.Ready;
                    
                    balanceController.ChangeBalance();

                    if (isFinalFreeSpin)
                    {
                        reelsState = ReelStateEnum.Stop;
                        freeSpinComponent.ShowFreeSpinPopup();
                        balanceController.EndFreeSpin();
                        isFinalFreeSpin = false;
                    }
                }
                if (balanceController.BalanceModel.FreeSpinCount != 0 && reelsState == ReelStateEnum.Ready)
                {
                    WatchFreeSpin();
                    StartCoroutine(FreeSpinCorotuine());
                }
            });
    }

    public void CorrectSpin(Transform reelT)
    {
        DOTween.Kill(reelT);
        var spinDistance = spinIteration * spinCount;
        var currentReelPosY = reelT.localPosition.y;
        var extraDistance = CalculateExtraDistance(currentReelPosY);
        var correctionDistance = currentReelPosY - extraDistance;
        var correctionDuration = extraDistance / -(spinDistance / spinDuration);
        reelT.DOLocalMoveY(correctionDistance, correctionDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                reelsState = ReelStateEnum.Stop;
                if (reelsDictionary[reelT].reelId == reels.Length && PreviewScatter())
                {
                    anticipationParticle.SetActive(true);
                    audioController.PlayLoopAudio(AudioType.SFX_Anticipation);
                    var extraSpinDistance = reelT.localPosition.y + spinIteration * 15;
                    var extraSpinDuration = extraSpinDistance / spinIteration / 4f;
                    reelT.DOLocalMoveY(extraSpinDistance, extraSpinDuration)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            ScrollStop(reelT);
                        });
                } 
                else
                {
                    ScrollStop(reelT);
                }
            });
    }

    private IEnumerator FreeSpinCorotuine()
    {
        if (balanceController.BalanceModel.CurrentPrize > 0)
        {
            yield return new WaitForSeconds(winLineDuration);
            StartSpin();
        }
        else
        {
            while (AllReelsStopped() == false)
            {
                yield return null;
            }
            StartSpin();
        }
    }

    public void ForceStopReels()
    {
        foreach (var reel in reels)
        {
            var reelT = reel.GetComponent<Transform>();
            CorrectSpin(reelT);
        }
    }

    private float CalculateExtraDistance(float currentReelPositionY)
    {
        var traveledDistance = -currentReelPositionY;
        var partOfUpperSymbol = traveledDistance % symbolHeight;
        var extraDistance = symbolHeight - partOfUpperSymbol;

        return extraDistance;
    }

    private void PrepareReel(Transform reelT)
    {
        var prevReelPosY = reelT.localPosition.y;
        var traveledReelDistance = -prevReelPosY;
        reelT.localPosition = new Vector3(reelT.localPosition.x, 0);
        var currentReel = reelsDictionary[reelT];
        currentReel.ResetPosition(traveledReelDistance);
        ReelStopped(currentReel, true);
    }

    private void ReelStopped(Reel reel, bool isStopped)
    {
        if(isStopped)
        {
            reel.reelStopped = true;
        } 
        else
        {
            reel.reelStopped = false;
        }
    }

    private bool AllReelsStopped()
    {
        if (reels[0].reelStopped &&
            reels[1].reelStopped &&
            reels[2].reelStopped)
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    private void WatchFreeSpin()
    {
        if (balanceController.BalanceModel.FreeSpinCount - 1 == 0)
        {
            isFinalFreeSpin = true;
        }
        else
        {
            isFinalFreeSpin = false;
        }
        reelsState = ReelStateEnum.Stop;
        balanceController.BalanceModel.FreeSpinCount -= 1;
    }

    private bool PreviewScatter()
    {
        var firstReel = reels[0].ScatterOnFinalScreen();
        var secondReel = reels[1].ScatterOnFinalScreen();
        if (firstReel && secondReel)
        {
            return true;
        }
        return false;
    }

    private void StartState()
    {
        if (balanceController.BalanceModel.FreeSpinCount == 0 && !isFinalFreeSpin)
        {
            reelsState = ReelStateEnum.Start;
        }
        else
        {
            reelsState = ReelStateEnum.Stop;
        }
    }

    private void CheckButtonState()
    {
        switch (reelsState)
        {
            case ReelStateEnum.Ready:
                ChangeButtonState(true, Vector3.one, false, Vector3.zero);
                break;
            case ReelStateEnum.Start:
                ChangeButtonState(false, Vector3.one, false, Vector3.zero);
                break;
            case ReelStateEnum.Spin:
                ChangeButtonState(false, Vector3.zero, true, Vector3.one);
                break;
            case ReelStateEnum.Stop:
                ChangeButtonState(false, Vector3.zero, false, Vector3.one);
                break;
        }
    }

    private void ChangeButtonState(bool startBtnInteractable, Vector3 startBtnScale, bool stopBtnInteractable, Vector3 stopBtnScale)
    {
        startBtn.interactable = startBtnInteractable;
        startBtn.transform.localScale = startBtnScale;
        stopBtn.interactable = stopBtnInteractable;
        stopBtn.transform.localScale = stopBtnScale;
    }
}

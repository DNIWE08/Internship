using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ReelSpinner : MonoBehaviour
{
    [SerializeField] private Reel[] reels;
    [SerializeField] private int spinCount;
    [SerializeField] private float spinDuration = 1;

    [SerializeField] public Button startBtn;
    [SerializeField] public Button stopBtn;

    private float spinIteration;
    private float symbolHeight;

    private Dictionary<Transform, Reel> reelsDictionary;

    private ReelStateEnum reelsState = ReelStateEnum.Ready;
    internal ReelStateEnum ReelsState { get => reelsState; set => reelsState = value; }


    private void Start()
    {
        symbolHeight = reels[1].ReelSymbols[1].GetComponent<RectTransform>().rect.height;
        spinIteration = -symbolHeight * reels[1].ReelSymbols.Length;

        reelsDictionary = new Dictionary<Transform, Reel>();
        for (var i = 0; i < reels.Length; i++)
        {
            var reelT = reels[i].transform;
            reelsDictionary.Add(reelT, reels[i]);
        }
    }

    private void Update()
    {
        CheckButtonState();
    }

    public void StartSpin()
    {
        WinLineChecker.ForceSpinStart();
        reelsState = ReelStateEnum.Start;
        for (int i = 0; i < reels.Length; i++)
        {
            var reelT = reels[i].transform;
            reels[i].isFinalSpin = false;
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
            }
            );
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
        var currentReelPosY = reelT.localPosition.y;
        var stoppingDistance = currentReelPosY - symbolHeight * 3;
        reelT.DOLocalMoveY(stoppingDistance, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if(reelsDictionary[reelT].reelId == 3)
                {
                    reelsState = ReelStateEnum.Ready;
                    WinLineChecker.StartCheckAnimation();
                }
                PrepareReel(reelT);
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
                ScrollStop(reelT);
                reelsState = ReelStateEnum.Stop;
                reelsDictionary[reelT].isFinalSpin = true;
            });
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
        var traveledDistance = 0 - currentReelPositionY;
        var partOfUpperSymbol = traveledDistance % symbolHeight;
        var extraDistance = symbolHeight - partOfUpperSymbol;

        return extraDistance;
    }

    private void PrepareReel(Transform reelT)
    {
        var prevReelPosY = reelT.localPosition.y;
        var traveledReelDistance = -(0 + prevReelPosY);
        reelT.localPosition = new Vector3(reelT.localPosition.x, 0);
        reelsDictionary[reelT].ResetPosition(traveledReelDistance);
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

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

    [SerializeField] public GameObject startBtn;
    [SerializeField] public GameObject stopBtn;

    private float spinIteration;

    private ReelStateEnum reelsState = ReelStateEnum.Ready;
    internal ReelStateEnum ReelsState { get => reelsState; set => reelsState = value; }

    private float symbolHeight;

    private void Start()
    {
        symbolHeight = reels[1].ReelSymbols[1].GetComponent<RectTransform>().rect.height;
        spinIteration = -symbolHeight * reels[1].ReelSymbols.Length;
    }

    private void Update()
    {
        CheckButtonState();
    }

    public void StartSpin()
    {
        WinLineChacker.ForceSpinStart();
        reelsState = ReelStateEnum.Start;
        for (int i = 0; i < reels.Length; i++)
        {
            var reelT = reels[i].transform;
            reelT.GetComponent<Reel>().isFinalSpin = false;
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
        //reelsState = ReelStateEnum.Stop;
        //reelT.GetComponent<Reel>().isFinalSpin = true;
        DOTween.Kill(reelT);
        var currentReelPosY = reelT.localPosition.y;
        var stoppingDistance = currentReelPosY - symbolHeight * 3;
        reelT.DOLocalMoveY(stoppingDistance, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if(reelT.GetComponent<Reel>().reelId == 3)
                {
                    reelsState = ReelStateEnum.Ready;
                    WinLineChacker.StartCheckAnimation();
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
                reelT.GetComponent<Reel>().isFinalSpin = true;
    });
    }

    public void ForceStopReels()
    {
        //DOTween.KillAll();
        foreach (var reel in reels)
        {
            //if(reel.isFinalSpin == false)
            //{
                var reelT = reel.GetComponent<Transform>();
                CorrectSpin(reelT);
            //}
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
        reelT.GetComponent<Reel>().ResetPosition(traveledReelDistance);
    }

    private void CheckButtonState()
    {
        if (reelsState == ReelStateEnum.Ready)
        {
            startBtn.GetComponent<Button>().interactable = true;
            startBtn.transform.localScale = Vector3.one;
            stopBtn.GetComponent<Button>().interactable = false;
            stopBtn.transform.localScale = Vector3.zero;
        }
        if (reelsState == ReelStateEnum.Start)
        {
            startBtn.GetComponent<Button>().interactable = false;
        }
        if (reelsState == ReelStateEnum.Spin)
        {
            startBtn.transform.localScale = Vector3.zero;
            stopBtn.transform.localScale = Vector3.one;
            stopBtn.GetComponent<Button>().interactable = true;
        }
        if (reelsState == ReelStateEnum.Stop)
        {
            stopBtn.GetComponent<Button>().interactable = false;
        }
        if (reelsState == ReelStateEnum.ForceStop)
        {
            stopBtn.GetComponent<Button>().interactable = false;
        }
        //if (reelsState == ReelStateEnum.EndAnimation)
        //{
        //    startBtn.GetComponent<Button>().interactable = false;
        //    startBtn.transform.localScale = Vector3.one;
        //    stopBtn.GetComponent<Button>().interactable = false;
        //    stopBtn.transform.localScale = Vector3.zero;
        //}
    }
}

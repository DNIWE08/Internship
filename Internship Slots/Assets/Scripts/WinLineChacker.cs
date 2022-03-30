using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class WinLineChacker : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;

    [SerializeField] private Reel[] reels;
    private WinLineConfig[] winLinesData;

    public static event Action OnReelsStop;

    private void Start()
    {
        winLinesData = gameConfig.WinLines;
        OnReelsStop += WinLinesAnimation;
    }

    public List<Transform> CheckWinLines()
    {
        List<Transform> winItems = new List<Transform>();
        Transform[] checkWinLine = new Transform[3];
        foreach (var line in winLinesData)
        {
            for(var i = 0; i < line.WinLine.Length; i++)
            {
                var currentReelSymbol = reels[i].EndReelSymbols[line.WinLine[i] - 1];
                checkWinLine[i] = currentReelSymbol;
            }
            if(checkWinLine[0].GetComponent<Image>().sprite.name == checkWinLine[1].GetComponent<Image>().sprite.name &&
                checkWinLine[1].GetComponent<Image>().sprite.name == checkWinLine[2].GetComponent<Image>().sprite.name)
            {
                winItems.Add(checkWinLine[0]);
                winItems.Add(checkWinLine[1]);
                winItems.Add(checkWinLine[2]);
            }
        }
        return winItems;
    }

    public void WinLinesAnimation()
    {
        var winSymbols = CheckWinLines();
        if(CheckWinLines().Count > 0)
        {
            foreach(var symbol in CheckWinLines())
            {
                var symbolParticle = symbol.GetChild(0);
                symbolParticle.gameObject.SetActive(true);
                symbol.DOScale(1.2f, 0.4f)
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        FillSymbols(winSymbols, Color.white);
                        symbolParticle.gameObject.SetActive(false);
                    });
                FillSymbols(winSymbols, Color.grey);
            }
        }

        foreach(var reel in reels)
        {
            reel.ClearEndReels();
        }
    }

    private void FillSymbols(List<Transform> winSymbols, Color color)
    {
        for (var i = 0; i < reels.Length; i++)
        {
            foreach (var reelSymbol in reels[i].ReelSymbols)
            {
                if (reelSymbol != winSymbols[i])
                {
                    reelSymbol.GetComponent<Image>().color = color;
                }
            }
        }
    }

    public static void StartCheckAnimation()
    {
        OnReelsStop?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class WinLineChecker : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Reel[] reels;
    private WinLineConfig[] winLinesData;
    private readonly int symbolOnReel = 3;

    [SerializeField] BalanceController balanceController;

    private Dictionary<Transform, Symbol> symbolsDictionary;
    private Dictionary<Sprite, float> prizeDictionary;

    public static event Action OnReelsStop;
    public static event Action OnForceSpinStart;

    private void Start()
    {
        prizeDictionary = new Dictionary<Sprite, float>();
        for (var i = 0; i < gameConfig.GameSprites.Length; i++)
        {
            var sprite = gameConfig.GameSprites[i];
            prizeDictionary.Add(sprite.SpriteImage, sprite.SpriteCost);
        }

        symbolsDictionary = new Dictionary<Transform, Symbol>();
        for (var i = 0; i < reels.Length; i++)
        {
            foreach (var reelSymbol in reels[i].ReelSymbols)
            {
                symbolsDictionary.Add(reelSymbol.SymbolRT, reelSymbol);
            }
        }

        winLinesData = gameConfig.WinLines;
        OnReelsStop += WinLinesAnimation;
        OnForceSpinStart += ResetWinAnimation;
    }

    public List<Transform> CheckWinLines()
    {
        List<Transform> winItems = new List<Transform>();
        Transform[] checkWinLine = new Transform[3];
        foreach (var line in winLinesData)
        {
            for (var i = 0; i < line.WinLine.Length; i++)
            {
                var currentReelSymbol = reels[i].EndReelSymbols[line.WinLine[i] - 1];
                checkWinLine[i] = currentReelSymbol;
            }
            if (symbolsDictionary[checkWinLine[0]].SymbolImage.sprite.name ==
                symbolsDictionary[checkWinLine[1]].SymbolImage.sprite.name &&
                symbolsDictionary[checkWinLine[1]].SymbolImage.sprite.name ==
                symbolsDictionary[checkWinLine[2]].SymbolImage.sprite.name)
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
        if (winSymbols.Count > 0)
        {
            var prize = GetWinPrize(winSymbols);
            balanceController.GetSpinPrize((int)prize);
            FillSymbols(Color.grey);
            AnimateSymbols(winSymbols);
        }

        foreach (var reel in reels)
        {
            reel.ClearEndReels();
        }
    }

    private void AnimateSymbols(List<Transform> winSymbols)
    {
        foreach (var symbol in winSymbols)
        {
            var symbolParticle = symbolsDictionary[symbol].SymbolParticle;
            var symbolImage = symbolsDictionary[symbol].SymbolImage;

            symbolParticle.SetActive(true);
            symbolImage.color = Color.white;

            symbol.DOScale(1.2f, 0.4f)
                .SetLoops(4, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    FillSymbols(Color.white);
                    symbolParticle.SetActive(false);
                });
        }
    }
    
    private void FillSymbols(Color color)
    {
        for (var i = 0; i < reels.Length; i++)
        {
            foreach (var reelSymbol in reels[i].ReelSymbols)
            {
                reelSymbol.SymbolImage.color = color;
            }
        }
    }

    private void ResetWinAnimation()
    {
        DOTween.KillAll();
        for (var i = 0; i < reels.Length; i++)
        {
            foreach (var reelSymbol in reels[i].ReelSymbols)
            {
                var symbolParticle = reelSymbol.SymbolParticle;
                symbolParticle.SetActive(false);

                reelSymbol.SymbolImage.color = Color.white;
                reelSymbol.transform.localScale = Vector3.one;
            }
        }
        balanceController.StopCoroutine();
        balanceController.PrepareCounter();
    }

    private float GetWinPrize(List<Transform> symbols)
    {
        float prize = 0;
        for(var i = 0; i < symbols.Count; i++)
        {
            var symbol = symbolsDictionary[symbols[i]].SymbolImage.sprite;
            var symbolCost = prizeDictionary[symbol];
            prize += symbolCost;
        }
        return prize / symbolOnReel;
    }

    public static void StartCheckAnimation()
    {
        OnReelsStop?.Invoke();
    }

    public static void ForceSpinStart()
    {
        OnForceSpinStart?.Invoke();
    }
}

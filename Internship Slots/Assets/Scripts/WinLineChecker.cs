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

    [SerializeField] private Text counterText;
    private float prize = 0f;
    private readonly int symbolOnReel = 3;

    private Dictionary<Transform, Symbol> symbolsDictionary;

    public static event Action OnReelsStop;
    public static event Action OnForceSpinStart;

    private void Start()
    {
        symbolsDictionary = new Dictionary<Transform, Symbol>();
        for(var i = 0; i < reels.Length; i++)
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
            for(var i = 0; i < line.WinLine.Length; i++)
            {
                var currentReelSymbol = reels[i].EndReelSymbols[line.WinLine[i] - 1];
                checkWinLine[i] = currentReelSymbol;
            }
            if(symbolsDictionary[checkWinLine[0]].SymbolImage.sprite.name ==
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
        if(winSymbols.Count > 0)
        {
            prize = GetWinPrize(winSymbols);
            StartCoroutine(CounterCorutine());
            FillSymbols(Color.grey);

            foreach (var symbol in winSymbols)
            {
                var symbolParticle = symbolsDictionary[symbol].SymbolParticle;
                symbolParticle.SetActive(true);
                symbolsDictionary[symbol].SymbolImage.color = Color.white;

                symbol.DOScale(1.2f, 0.4f)
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        FillSymbols(Color.white);
                        symbolParticle.SetActive(false);
                    });
            }
        }

        foreach(var reel in reels)
        {
            reel.ClearEndReels();
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
        prize = 0f;
        counterText.text = prize.ToString();
    }

    private float GetWinPrize(List<Transform> symbols)
    {
        var winSymbolName = symbolsDictionary[symbols[0]].SymbolImage.sprite.name;
        float prize = 0;
        for(var i = 0; i < gameConfig.GameSprites.Length; i++)
        {
            var cfg = gameConfig.GameSprites;
            if (cfg[i].SpriteImage.name == winSymbolName)
            {
                prize = cfg[i].SpriteCost * (symbols.Count / symbolOnReel);
            }
        }
        return prize;
    }

    public static void StartCheckAnimation()
    {
        OnReelsStop?.Invoke();
    }

    public static void ForceSpinStart()
    {
        OnForceSpinStart?.Invoke();
    }

    private IEnumerator CounterCorutine()
    {
        for(var i = 0; i <= prize; i++)
        {
            counterText.text = i.ToString();
            //yield return null;
            yield return new WaitForSeconds(0.005f);
        }
    }
}

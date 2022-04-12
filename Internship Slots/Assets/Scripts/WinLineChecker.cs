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

    private Dictionary<Transform, Symbol> symbolsImage;

    public static event Action OnReelsStop;
    public static event Action OnForceSpinStart;

    private void Start()
    {
        symbolsImage = new Dictionary<Transform, Symbol>();
        for(var i = 0; i < reels.Length; i++)
        {
            foreach (var reelSymbol in reels[i].ReelSymbols)
            {
                symbolsImage.Add(reelSymbol.SymbolRT, reelSymbol);
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
            if(checkWinLine[0].GetComponentInChildren<Image>().sprite.name == checkWinLine[1].GetComponentInChildren<Image>().sprite.name &&
                checkWinLine[1].GetComponentInChildren<Image>().sprite.name == checkWinLine[2].GetComponentInChildren<Image>().sprite.name)
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
            prize = GetWinPrize(winSymbols[0]);
            StartCoroutine(CounterCorutine());
            foreach(var symbol in CheckWinLines())
            {
                var symbolParticle = symbolsImage[symbol].SymbolParticle;
                symbolParticle.SetActive(true);


                symbol.DOScale(1.2f, 0.4f)
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        FillSymbols(winSymbols, Color.white);
                        symbolParticle.SetActive(false);
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
                if (reelSymbol.SymbolRT != winSymbols[i])
                {
                    reelSymbol.SymbolImage.color = color;
                }
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

    private float GetWinPrize(Transform symbol)
    {
        var winSymbolName = symbolsImage[symbol].SymbolImage.sprite.name;
        float prize = 0;
        for(var i = 0; i < gameConfig.GameSprites.Length; i++)
        {
            var cfg = gameConfig.GameSprites;
            if (cfg[i].SpriteImage.name == winSymbolName)
            {
                prize = cfg[i].SpriteCost;
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

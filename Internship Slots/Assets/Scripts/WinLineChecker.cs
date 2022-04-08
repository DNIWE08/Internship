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

    private Dictionary<Transform, Transform> symbolsImage;

    public static event Action OnReelsStop;
    public static event Action OnForceSpinStart;

    private void Start()
    {
        symbolsImage = new Dictionary<Transform, Transform>();
        for(var i = 0; i < reels.Length; i++)
        {
            foreach (var reelSymbol in reels[i].ReelSymbols)
            {
                var symbolImage = reelSymbol.Find("Image");
                symbolsImage.Add(reelSymbol, symbolImage);
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
            if(checkWinLine[0].Find("Image").GetComponent<Image>().sprite.name == checkWinLine[1].Find("Image").GetComponent<Image>().sprite.name &&
                checkWinLine[1].Find("Image").GetComponent<Image>().sprite.name == checkWinLine[2].Find("Image").GetComponent<Image>().sprite.name)
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
                var symbolParticle = symbol.GetChild(0);
                symbolParticle.gameObject.SetActive(true);


                symbolsImage[symbol].DOScale(1.2f, 0.4f)
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
                    symbolsImage[reelSymbol].GetComponent<Image>().color = color;
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
                var symbolParticle = reelSymbol.GetChild(0);
                symbolParticle.gameObject.SetActive(false);
                symbolsImage[reelSymbol].GetComponent<Image>().color = Color.white;
                symbolsImage[reelSymbol].transform.localScale = Vector3.one;
            }
        }
        counterText.text = "0";
        prize = 0f;
    }

    private float GetWinPrize(Transform symbol)
    {
        var winSymbolName = symbolsImage[symbol].GetComponent<Image>().sprite.name;
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

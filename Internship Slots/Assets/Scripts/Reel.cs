using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    [SerializeField] private RectTransform mainCanvasRT;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] public int reelId;

    [SerializeField] private Symbol[] reelSymbols;
    private List<Transform> endReelSymbols;

    private Dictionary<Transform, Symbol> symbolsDictionary;

    [SerializeField] private int symbolsOnReel;
    private int finalScreenNumber = 0;
    private int currentFinalSymbol = 0;
    internal bool isFinalSpin = false;

    [SerializeField] private float endPosition;
    private float mainCanvasScale;
    private float symbolHeight;

    internal Symbol[] ReelSymbols { get => reelSymbols; }
    internal List<Transform> EndReelSymbols => endReelSymbols;


    private void Start()
    {
        symbolHeight = reelSymbols[0].GetComponent<RectTransform>().rect.height;
        mainCanvasScale = mainCanvasRT.lossyScale.y;
        endReelSymbols = new List<Transform>();

        symbolsDictionary = new Dictionary<Transform, Symbol>();
        for(int i = 0; i < reelSymbols.Length; i++)
        {
            symbolsDictionary.Add(reelSymbols[i].SymbolRT, reelSymbols[i]);
        }

        foreach (var symbol in reelSymbols)
        {
            ChangeSprite(symbol.SymbolRT);
        }
    }

    private void Update()
    {
        for (var i = 0; i < reelSymbols.Length; i++)
        {
            var reelT = reelSymbols[i].transform;

            if (reelT.position.y <= -endPosition * mainCanvasScale)
            {
                MoveTop(reelT);
                ChangeSprite(reelT);
            }
        }
    }

    private void MoveTop(Transform reelT)
    {
        var topSymbolPosition = reelT.localPosition.y + symbolHeight * reelSymbols.Length;
        var topPosition = new Vector3(reelT.localPosition.x, topSymbolPosition);
        reelT.localPosition = topPosition;
    }

    private void ChangeSprite(Transform reelT)
    {
        if (isFinalSpin)
        {
            symbolsDictionary[reelT].SymbolImage.sprite = GetFinalSprite();
            if (endReelSymbols.Count < symbolsOnReel)
            {
                endReelSymbols.Add(reelT);
            }
        }
        else
        {
            symbolsDictionary[reelT].SymbolImage.sprite = GetRandomSprite();
        }
    }

    private Sprite GetFinalSprite()
    {
        var finalScreenItemIndex = currentFinalSymbol + (reelId - 1) * symbolsOnReel;
        var currentFinalScreen = gameConfig.FinalScreens[finalScreenNumber].FinalScreenData;
        if (finalScreenItemIndex >= currentFinalScreen.Length)
        {
            finalScreenItemIndex = 0;
        }
        var newSymbol = gameConfig.GameSprites[currentFinalScreen[finalScreenItemIndex]];
        currentFinalSymbol++;
        return newSymbol.SpriteImage;
    }

    private Sprite GetRandomSprite()
    {
        int randomSymbol = Random.Range(0, gameConfig.GameSprites.Length);
        var sprite = gameConfig.GameSprites[randomSymbol].SpriteImage;
        return sprite;
    }

    public void ResetPosition(float spinnerPosition)
    {
        ResetValues();
        foreach (var symbol in reelSymbols)
        {
            var reelPos = symbol.transform.localPosition;
            var correction = Mathf.Round(reelPos.y - spinnerPosition);
            var correctedPos = new Vector3(reelPos.x, correction);
            symbol.transform.localPosition = correctedPos;
        }
    }

    private void ResetValues()
    {
        currentFinalSymbol = 0;
        if (finalScreenNumber < gameConfig.FinalScreens.Length - 1)
        {
            finalScreenNumber++;
        }
        else
        {
            finalScreenNumber = 0;
        }
    }

    public void ClearEndReels()
    {
        endReelSymbols.Clear();
    }
}

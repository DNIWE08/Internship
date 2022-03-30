using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Game Config", menuName = "Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private GameSprite[] gameSprites;

    [SerializeField] private FinalScreen[] finalScreens;

    [SerializeField] private WinLineConfig[] winLines;

    public GameSprite[] GameSprites => gameSprites;

    public FinalScreen[] FinalScreens => finalScreens;

    public WinLineConfig[] WinLines => winLines;
}


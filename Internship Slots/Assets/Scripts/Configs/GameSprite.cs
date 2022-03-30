using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Game Sprite", menuName = "Game Sprite")]
public class GameSprite : ScriptableObject
{
    [SerializeField] private Sprite spriteImage;
    [SerializeField] private int spriteCost;

    public Sprite SpriteImage => spriteImage;

    public float SpriteCost => spriteCost;
}


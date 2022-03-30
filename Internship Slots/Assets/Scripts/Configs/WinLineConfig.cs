using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Win Config", menuName = "Win Config")]
public class WinLineConfig : ScriptableObject
{
    [SerializeField] private int[] winLine;


    public int[] WinLine => winLine;

}


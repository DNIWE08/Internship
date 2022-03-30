using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Final Screen", menuName = "Final Screen")]
public class FinalScreen : ScriptableObject
{
    [SerializeField] private int[] finalScreenData;

    public int[] FinalScreenData => finalScreenData;
}


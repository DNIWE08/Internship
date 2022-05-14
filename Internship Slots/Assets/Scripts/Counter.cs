using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [SerializeField] private Text counterText;
    public Text CounterText { get => counterText; set => counterText = value; }
}

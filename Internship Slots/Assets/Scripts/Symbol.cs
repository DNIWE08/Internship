using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [SerializeField] Image symbolImage;
    [SerializeField] GameObject symbolParticle;

    internal Image SymbolImage { get => symbolImage; }

    internal GameObject SymbolParticle { get => symbolParticle; }

    internal Transform SymbolRT { get => gameObject.transform; }    

}

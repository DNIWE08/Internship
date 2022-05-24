using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioController audioController;

    #region UnityFunctions
    private void Start()
    {
        audioController.PlayAudio(AudioType.BG_01);
    }

    #endregion

    #region Public Functions
    public void BtnPress()
    {
        audioController.PlayAudio(AudioType.SFX_PressBtn);
    }
    #endregion
}

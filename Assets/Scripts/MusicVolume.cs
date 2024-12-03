using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolume : MonoBehaviour
{
    public bool restartAudio = false;
    public string songName = "calmbgm";
    private void Start()
    {
        if (MusicManager.instance != null && MusicManager.instance.hasClip(songName))
        {
            MusicManager.instance.PlayMusic(songName, restartAudio);
        }
        else
        {
            Debug.LogWarning($"Music \"{songName}\" not found");
        }
    }

    private void OnValidate()
    {
    }
}

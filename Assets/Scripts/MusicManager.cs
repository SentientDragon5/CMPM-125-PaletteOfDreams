using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.LogWarning("More than one instance of MusicManager");
            Destroy(this.gameObject);
        }
    }

    public AudioSource musicSource;

    [Serializable]
    public struct AudioClipNamed
    {
        public string name;
        public AudioClip clip;
    }
    public List<AudioClipNamed> audioClips = new();
    public void PlayMusic(string musicName, bool restart = false)
    {
        if (musicSource.clip.name != musicName)
        {
            musicSource.clip = audioClips.Find((clip)=> { return clip.name == musicName; }).clip;
        }
        else
        {
            if (restart)
            {
                musicSource.Stop();
                musicSource.Play();
            }
        }
    }

    public bool hasClip(string musicName)
    {
        return audioClips.Any((clip) => clip.name == musicName);
    }
    
}

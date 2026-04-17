using OverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;
    public InGameSoundManager soundManager;
    public float bgmVolume=1;
    public float effectVolume=1;
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        soundManager=InGameSoundManager.Instance;
    }

    public void UpdateVolumToAudioSource()
    {
        soundManager.UpdateVolume();
    }
}

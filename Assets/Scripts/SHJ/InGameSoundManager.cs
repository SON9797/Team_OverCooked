using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OverCooked
{
    public class InGameSoundManager : MonoBehaviour, IInGameSoundManager
    {
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _bgmSource;

        [SerializeField] private SoundLibrarySO _library;

        private Dictionary<SFXType, SoundData> _sfxDictionary;

        private void Awake()
        {
            if (_library != null)
            {
                _sfxDictionary = _library.GetDictionary();
            }
        }

        public void PlaySFX(SFXType sfxType)
        {
            if (sfxType == SFXType.None)
            {
                return;
            }

            if (_sfxDictionary.TryGetValue(sfxType, out SoundData soundData))
            {
                soundData.Play(_sfxSource);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || _bgmSource == null)
            {
                return;
            }

            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
            {
                return;
            }

            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        public void StopBGM()
        {
            if (_bgmSource != null && _bgmSource.isPlaying)
            {
                _bgmSource.Stop();
            }
        }

    }
}

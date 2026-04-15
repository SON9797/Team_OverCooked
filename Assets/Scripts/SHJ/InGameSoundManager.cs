using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OverCooked
{
    public class InGameSoundManager : MonoBehaviour, IInGameSoundManager, ICommonSoundManager
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

        public void StopAllSounds()
        {
            if (_bgmSource != null && _bgmSource.isPlaying)
            {
                _bgmSource.Stop();
            }

            if (_sfxSource != null && _sfxSource.isPlaying)
            {
                _sfxSource.Stop();
            }
        }

        public void StopSFX(SFXType sfxType)
        {
            if (sfxType == SFXType.None || _sfxSource == null)
            {
                return;
            }

            if (_sfxDictionary.TryGetValue(sfxType, out SoundData soundData))
            {
                if (_sfxSource.clip == soundData.Clip && _sfxSource.isPlaying)
                {
                    _sfxSource.Stop();
                }
            }
        }
    }
}

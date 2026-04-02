using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverCooked
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundData")]
    public class SoundData : ScriptableObject
    {
        public SFXType Type;
        public AudioClip Clip;

        [Range(0f, 1f)] public float Volume = 1f;
        [Range(0.1f, 3f)] public float Pitch = 1f;

        public bool UseRandomPitch = false;
        [Range(0.1f, 3f)] public float PitchRandomRange = 0.1f;

        public void Play(AudioSource source)
        {
            if (Clip == null)
            {
                return;
            }

            source.clip = Clip;
            source.volume = Volume;
            source.pitch = UseRandomPitch ? Pitch + Random.Range(-PitchRandomRange, PitchRandomRange) : Pitch;

            source.PlayOneShot(Clip);
        }
    }
}

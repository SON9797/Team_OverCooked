using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverCooked
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
    public class SoundLibrarySO : ScriptableObject
    {
        public List<SoundData> SoundEffect;

        public Dictionary<SFXType, SoundData> GetDictionary()
        {
            var dict = new Dictionary<SFXType, SoundData>();

            foreach (var effect in SoundEffect)
            {
                if (effect != null && !dict.ContainsKey(effect.Type))
                {
                    dict.Add(effect.Type, effect);
                }
            }
            return dict;
        }
    }
}

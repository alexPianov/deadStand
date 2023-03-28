using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ItemSFX : MonoBehaviour
    {
        List<string> sfx = new List<string> 
        { "S_Use", "S_Hit", "S_Sld", "S_Swg", "S_Fir", "S_Ath", "S_Dth" };

        public enum Sounds
        { Use, Hit, Sld, Swg, Fir, Ath, Dth };

        private AudioSource _audioSource;

        public void SetAudioSource(AudioSource source)
        {
            _audioSource = source;
        }

        public AudioSource GetAudioSource()
        {
            return _audioSource;
        }

        public void SetItemFX(ItemInfo info)
        {
            var fx = info.GetTypedData(ItemInfo.DataType.FX);

            if (fx == null) 
            { Debug.LogWarning(info.itemName + " doesn't contains FX key"); return; };

            for (var i = 0; i < sfx.Count; i++)
            {
                SetSounds(fx, sfx[i]);
            }
        }

        Dictionary<string, List<AudioClip>> sfxCollection = new ();
        private async void SetSounds(Dictionary<string, string> customData, string key)
        {
            var soundsString = GetStatString(customData, key);

            if (string.IsNullOrEmpty(soundsString)) return;

            var soundsList = DataHandler.SplitString(soundsString);

            List<AudioClip> clips = new List<AudioClip>();

            if (soundsList.Count == 0) return;

            for (int i = 0; i < soundsList.Count; i++)
            {
                if (string.IsNullOrEmpty(soundsList[i])) continue;

                var sound = await AddressablesHandler.Load<AudioClip>(soundsList[i]);

                clips.Add(sound);
            }

            sfxCollection.Add(key, clips);
        }

        private string GetStatString(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValue(customData, statName);
        }

        public void PlaySound(Sounds sound)
        {
            var key = "S_" + sound;
            sfxCollection.TryGetValue(key, out List<AudioClip> list);
            if (list == null) return;
            if (list.Count == 0) return;
            var clip = list[Random.Range(0, list.Count - 1)];
            PlayClip(clip);
        }

        public void PlayClip(AudioClip clip, float volume = 1)
        {
            _audioSource.pitch = RandomValue(0.1f);
            _audioSource.PlayOneShot(clip, RandomValue(0.2f, volume));
        }

        private float RandomValue(float amplitude, float volume = 1)
        {
            return Random.Range(volume - amplitude, volume + amplitude);
        }
    }
}

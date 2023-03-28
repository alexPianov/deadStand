using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Playstel
{
    public class UiMusicMute : MonoBehaviour
    {
        public AudioMixer MusicMixer;

        private float normalValue;
        
        private void Start()
        {
            MusicMixer.GetFloat("Gain", out float value);
            normalValue = value;
            MusicMixer.SetFloat("Gain", value / 3);
        }

        private void OnDestroy()
        {
            MusicMixer.SetFloat("Gain", normalValue);
        }
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class CacheAudio : MonoBehaviour
    {
        public AudioSource soundSource;
        public AudioSource musicSource;
        public AudioListener AudioListener;

        public enum MenuSound
        {
            OnSwitchUi, OnBack, OnProcess, OnToggle, OnInput, OnCatalog,
            OnPurchase, OnCasinoSpin, OnGetItem, 
            OnGetStar, 
            
            OnTakeToken, 
            OnLootTake, OnLootDrop, OnSlotMove, 
            OnInventoryOpen, OnInventoryClose, OnCrateOpen, OnCrateClose, 
            OnTakeWeapon, OnAimOn, OnAimOff, OnUse, 
            OnCrush, OnSplash, OnFrag, OnNewLevel, OnDead, OnDamage,
            
            OnError
        }
        
        public enum Music
        {
            Loading, Menu, Lobby, Round, Cutscene, Null
        }

        public void ActiveAudioListener(bool state)
        {
            if(AudioListener) AudioListener.enabled = state;
        }

        private MenuSound currentSound;
        public async void Play(MenuSound sound, bool playOverLast = true, 
            float pitch = 0.05f, bool playWithDupe = false)
        {
            if (!playWithDupe)
            {
                if (soundSource.isPlaying && currentSound == sound) return;
            }

            if (!playOverLast)
            {
                if (soundSource.isPlaying) return;
            }
            
            var clip = await AddressablesHandler.Load<AudioClip>(sound.ToString());

            if(!clip)
            {
                Debug.Log("Failed to find sound " + sound);
                return;
            }

            currentSound = sound;
            soundSource.pitch = RandomValue(pitch);
            soundSource.PlayOneShot(clip, RandomValue(0.1f));
        }
        
        private float RandomValue(float amplitude)
        {
            return Random.Range(1 - amplitude, 1 + amplitude);
        }

        private AudioClip currentClip;
        public async UniTask PlayMusic(Music music, string suffix = null)
        {
            if (music == Music.Null)
            {
                currentClip = null;
                musicSource.clip = null;
                musicSource.Stop(); 
                return;
            }

            var musicClip = await AddressablesHandler.Load<AudioClip>(music + suffix);
            
            PlayMusic(musicClip);
        }

        public void PlayMusic(AudioClip musicClip)
        {
            if(currentClip == musicClip) return;
            
            currentClip = musicClip;
            musicSource.Stop();
            musicSource.clip = musicClip;
            
            if (musicClip) musicSource.Play(); 
        }
    }
}

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class CacheSoundClips : MonoBehaviour
    {
        #region Soundtrack

        [Header("Music")]
        public AudioClip levelMusic;
        private const int roundSoundtracksCount = 1;

        public async UniTask CacheRoundSoundtracks(string season)
        {
            levelMusic = await AddressablesHandler.Load<AudioClip>("Round_" + season);
        }

        #endregion

        #region Hit

        public List<AudioClip> hitFleshSounds = new();
        public List<AudioClip> hitSolidSounds = new();
        private int hitFleshSoundsCount = 8;
        private int hitSolidSoundsCount = 6;
        
        public enum HitType
        {
            Flesh, Solid
        }
        
        public async UniTask CacheHitSounds()
        {
            for (var i = 1; i < hitFleshSoundsCount; i++)
            {
                var soundClip = await AddressablesHandler
                    .Load<AudioClip>(KeyStore.SFX_HIT_FLESH + "_" + i);
                
                hitFleshSounds.Add(soundClip);
            }
            
            for (var i = 1; i < hitSolidSoundsCount; i++)
            {
                var soundClip = await AddressablesHandler
                    .Load<AudioClip>(KeyStore.SFX_HIT_SOLID + "_" + i);
                
                hitSolidSounds.Add(soundClip);
            }
        }

        public AudioClip GetRandomHitSound(HitType hitType)
        {
            if (hitType == HitType.Flesh)
            {
                return hitFleshSounds[Random.Range(0, hitFleshSounds.Count)];
            }
            
            if (hitType == HitType.Solid)
            {
                return hitSolidSounds[Random.Range(0, hitSolidSounds.Count)];
            }

            return null;
        }

        #endregion

        #region Reload

        private Dictionary<string, List<AudioClip>> reloadClips = new();

        public async UniTask CacheReloadSounds()
        {
            WeaponSubclassSounds(ReloadType.Cock, 4);
            WeaponSubclassSounds(ReloadType.Load, 4);
            WeaponSubclassSounds(ReloadType.Unload, 4);
            
            AddReloadSounds(ReloadType.Load, 2, ItemInfo.Subclass.Piercing);
            
            AddReloadSounds(ReloadType.RevolverShells, 2);
            AddReloadSounds(ReloadType.RevolverCock, 4);
            AddReloadSounds(ReloadType.FillCartridge, 4);
            AddReloadSounds(ReloadType.DryFire, 4);
            AddReloadSounds(ReloadType.CompletionSound);
        }

        private void WeaponSubclassSounds(ReloadType reloadType, int clipsNumbers)
        {
            AddReloadSounds(reloadType, clipsNumbers, ItemInfo.Subclass.Pistol);
            AddReloadSounds(reloadType, clipsNumbers, ItemInfo.Subclass.SMG);
            AddReloadSounds(reloadType, clipsNumbers, ItemInfo.Subclass.Shotgun);
            AddReloadSounds(reloadType, clipsNumbers, ItemInfo.Subclass.Autogun);
            AddReloadSounds(reloadType, clipsNumbers, ItemInfo.Subclass.Rifle);
            AddReloadSounds(reloadType, clipsNumbers, ItemInfo.Subclass.Heavy);
        }

        private async void AddReloadSounds(ReloadType reloadType, int clipsNumber = 1, ItemInfo.Subclass weaponSubclass = ItemInfo.Subclass.Null)
        {
            var sounds = new List<AudioClip>();

            string soundAddress;
            
            if (weaponSubclass == ItemInfo.Subclass.Null)
            {
                soundAddress = reloadType.ToString();
            }
            else
            {
                soundAddress = reloadType + "_" + weaponSubclass;
            }

            for (var i = 1; i < clipsNumber + 1; i++)
            {
                var clip = await AddressablesHandler.Load<AudioClip>(soundAddress + "_" + i);
                sounds.Add(clip);
            }

            reloadClips.Add(soundAddress, sounds);
        }
        
        public enum ReloadType
        {
            Load, Unload, Cock, RevolverCock, RevolverShells, 
            FillCartridge, DryFire, CompletionSound
        }
        
        public AudioClip GetReloadSound(ReloadType sound, ItemInfo.Subclass weaponSubclass = ItemInfo.Subclass.Null)
        {
            string soundAddress;
            
            if (weaponSubclass == ItemInfo.Subclass.Null)
            {
                soundAddress = sound.ToString();
            }
            else
            {
                soundAddress = sound + "_" + weaponSubclass;
            }

            if (!reloadClips.ContainsKey(soundAddress)) return null;
            
            reloadClips.TryGetValue(soundAddress, out List<AudioClip> list);

            var clipNumber = Random.Range(0, list.Count);

            if (list[clipNumber] == null) return list[0];
            
            return list[clipNumber];
        }

        #endregion
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitSFX : MonoBehaviour
    {
        List<string> sounds = new List<string> { "S_Spw", "S_Dmg", "S_Dth", "S_Dec", "S_Atk",
        "S_Brn", "S_Shk", "S_Psn", "S_Blw", "S_Exp", "S_Hea", "S_Say" };

        public enum Sounds
        {
            Spw, Dmg, Dth, Dec, Atk, Brn, Shk, Psn, Blw, Exp, Hea, Say
        }

        public bool disableSounds;

        Dictionary<string, List<AudioClip>> soundCollection = new ();

        [Inject] private CacheItemInfo _cacheItemInfo;

        private const string _setupKey = "Setup";
        private string _recentSfxSetup;
        public void SetUnitSFX(ItemInfo itemInfo)
        {
            var setupName = DataHandler.GetUnsafeValue
                (itemInfo.GetUnsafeCustomData(), _setupKey);
            
            if(_recentSfxSetup == setupName) return;
            _recentSfxSetup = setupName;
            
            if(disableSounds) return;
            
            var customData = _cacheItemInfo
                .GetItemInfo(setupName, ItemInfo.Catalog.Setup, ItemInfo.Class.Unit)
                .GetTypedData(ItemInfo.DataType.FX);

            if (customData == null) { Debug.Log(setupName + " doesn't contains FX key"); return; };

            foreach (var sound in sounds)
            {
                if (soundCollection.ContainsKey(sound)) continue;
                AddSoundsToCollection(customData, sound);
            }
        }

        private async void AddSoundsToCollection(Dictionary<string, string> customData, string soundListName)
        {
            var soundsString = GetStatString(customData, soundListName);

            if (string.IsNullOrEmpty(soundsString)) return;

            var soundsList = DataHandler.SplitString(soundsString);
            List<AudioClip> clips = new List<AudioClip>();
            
            foreach (var soundName in soundsList)
            {
                var sound = await AddressablesHandler.Load<AudioClip>(soundName);
                clips.Add(sound);
            }

            soundCollection.Add(soundListName, clips);
        }
        private string GetStatString(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValue(customData, statName);
        }

        public AudioClip GetRandomSound(Sounds sounds)
        {
            if (soundCollection.TryGetValue("S_" + sounds, out List<AudioClip> clips))
            {
                var num = Random.Range(0, clips.Count - 1);
                return clips[num];
            }

            return null;
        }
    }
}

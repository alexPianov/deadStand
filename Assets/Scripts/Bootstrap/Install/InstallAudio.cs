using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Install/Audio")]
    public class InstallAudio : ScriptableObjectInstaller<InstallAudio>
    {   
        private List<string> _soundNames = new List<string> 
        { "OnClose", "OnMenuButton", "OnError", "OnInput", "OnStartPurchase", 
        "OnSuccessPurchase", "OnRemove", "OnPick", "MusicStat", "MusicMenu", "MusicRound" };
        
        private Dictionary<string, AudioClip> _audioCollection = new ();

        public Dictionary<string, AudioClip> GetAudio()
        {
            return _audioCollection;
        }

        public async UniTask Install()
        {
            foreach (var sound in _soundNames)
            {
                await SetSounds(sound);
            }
        }

        private async UniTask SetSounds(string key)
        {
            var soundValue = await AddressablesHandler.Load<AudioClip>(key);
            _audioCollection.Add(key, soundValue);
        }
    }
}
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class RoomStartCutsceneLoader : MonoBehaviour
    {
        public RoomSceneEnter RoomSceneEnter;
        public RoomStartCutscene RoomStartCutscene;

        [Inject] private CacheAudio _cacheAudio;
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        public async void Cutscene()
        {
            _cacheAudio.PlayMusic(CacheAudio.Music.Null);
            _cacheAudio.Play(CacheAudio.MenuSound.OnSplash);
                
            await UniTask.Delay(200);

            RoomStartCutscene.StartCutscene();
            RoomSceneEnter.DisableLoadingSceen();
        }
        
        
        private bool CutsceneIsAlreadyShowed()
        {
            var cutscenes = GetCutscenesInfo();

            cutscenes.TryGetValue(_cacheUserSettings.pickedSeason, out var value);
            
            if (string.IsNullOrEmpty(value)) return false;
            
            var cutsceneIsAlreadyShowed = bool.Parse(value);

            return cutsceneIsAlreadyShowed;
        }

        public void SaveCutsceneWatchEvent()
        {
            Debug.Log("SaveCutsceneWatchEvent");
            var cutscenes = GetCutscenesInfo();

            if (cutscenes.ContainsKey(_cacheUserSettings.pickedSeason))
            {
                cutscenes.Remove(_cacheUserSettings.pickedSeason);
            }
            
            cutscenes.Add(_cacheUserSettings.pickedSeason, "true");
            
            var args = new
            {
                CutscenesData = DataHandler.DictionaryToString(cutscenes)
            };

            PlayFabHandler.ExecuteCloudScript(PlayFabHandler.Function.CutsceneIsWatched, args);
        }

        private Dictionary<string, string> GetCutscenesInfo()
        {
            var cutscenesString = _cacheUserInfo.data.GetUserData(UserData.UserDataType.Cutscenes);

            if (string.IsNullOrEmpty(cutscenesString)) return new Dictionary<string, string>();

            var cutscenes = DataHandler.Deserialize(cutscenesString);
            
            return cutscenes;
        }
    }
}
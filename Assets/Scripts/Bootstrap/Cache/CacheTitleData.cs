using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playstel
{
    public class CacheTitleData : MonoBehaviour
    {
        private Dictionary<ObscuredString, ObscuredString> _titleData = new();
        
        public enum TitleDataKey
        {
            AnarchySeasonLevels, AnarchySeasonMaterials, 
            SpinPrice, CasinoItems, AppVersion, AnarchySeasonUnitStats
        }

        public async UniTask InstallTitleData()
        {
            var result = await PlayFabHandler.GetTitleData();
            
            if (result == null)
            {
                Debug.Log("Failed to find title data");
            }
            else
            {
                _titleData = DataHandler.ConvertToSafeData(result.Data);
            }
        }

        public ObscuredString GetTitleData(TitleDataKey key)
        {
            return GetTitleData(key.ToString());
        }

        public ObscuredInt GetTitleDataInt(TitleDataKey key)
        {
            var result = GetTitleData(key.ToString());
            
            if(string.IsNullOrEmpty(result)) return 0;

            return int.Parse(result);
        }
        
        public ObscuredString GetTitleData(string key)
        {
            _titleData.TryGetValue(key, out var value);
            
            if (value == null) 
            {
                Debug.Log("Failed to find title data value by key " + key);
                return null;
            }
            
            return value;
        }

    }
}

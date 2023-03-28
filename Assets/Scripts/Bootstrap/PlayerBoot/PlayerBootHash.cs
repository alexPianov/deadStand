using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using PlayFab.Json;
using Playstel.Bootstrap;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class PlayerBootHash : MonoBehaviourPun
    {
        public ItemInfo.Subclass startWeapon = ItemInfo.Subclass.Pistol;

        public CacheTitleData.TitleDataKey unitTitleData = 
            CacheTitleData.TitleDataKey.AnarchySeasonUnitStats;
    
        [Inject] private CacheTitleData _cacheTitleData;
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private BootstrapInstaller _bootstrapInstaller;

        private Dictionary<string, string> customDataTitle = new();
        
        public enum TitleValue
        {
            Health, MaxEndurance, RunSpeed, SprintSpeed
        }
        
        public async UniTask InstallPlayerNetworkCustomProperties(int fraction)
        {
            if(!PhotonNetwork.InRoom) return;
            
            PhotonHandler.ClearNetworkCustomProperties();
            
            InitializeTitleData();

            SetFraction(fraction);
            SetCustomSkin();
            await SetItems();
            SetResources();
            SetExperience();
            SetMaxValues();
            SetMove();
        }
        
        private void InitializeTitleData()
        {
            customDataTitle = GetCustomData(unitTitleData);
        }

        private void SetFraction(int fraction)
        {
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Fraction, fraction);
        }
        
        private void SetCustomSkin()
        {
            string _unitSkin = _cacheUserInfo.data.GetUserData(UserData.UserDataType.UnitSkin);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Skin, _unitSkin);
        }

        private async UniTask SetItems()
        {
            var userCharacterItems = await GetSessionItems();
            var userWeapons = GetStartWeapon();
            
            if (userWeapons == null || userCharacterItems == null)
            {
                PhotonNetwork.LeaveRoom();
                Debug.Log("Failed to find user items");
                return;
            } 

            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Character, userCharacterItems);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Weapons, userWeapons);
        }

        private void SetResources()
        {
            var _maxHealth = GetTitleValue(TitleValue.Health);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Health, _maxHealth);
            
            var _startLives = GetData(UserData.UserDataType.Lives);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Lives, _startLives);

            var _localCurrency = GetData(UserData.UserDataType.Currency);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.LocalCurrency, _localCurrency);

            var _mainCurrency = (int)_cacheUserInfo.payload.GetPlayerMainCurrency();
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.MainCurrency, _mainCurrency);
        }
        
        private void SetExperience()
        {
            var _currentExp = GetUserStatistic(UserPayload.Statistics.Experience);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Experience, _currentExp);

            var _currentLvl = GetUserStatistic(UserPayload.Statistics.Level);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Level, _currentLvl);
        }

        private void SetMove()
        {
            var _runSpeed = GetTitleValue(TitleValue.RunSpeed);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.RunSpeed, _runSpeed);
            
            var _sprintSpeed = GetTitleValue(TitleValue.SprintSpeed);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.SprintSpeed, _sprintSpeed);
        }

        private void SetMaxValues()
        {
            var _maxHealth = GetTitleValue(TitleValue.Health);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.MaxHealth, _maxHealth);
            
            var _maxBagSlots = GetData(UserData.UserDataType.Bag);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.BagSlots, _maxBagSlots);
            
            var _maxLives = GetData(UserData.UserDataType.Lives);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.MaxLives, _maxLives);
            
            var _maxExperience = GetUserStatistic(UserPayload.Statistics.MaxExperience);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.MaxExperience, _maxExperience);
            
            var _maxEndurance = GetTitleValue(TitleValue.MaxEndurance);
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.MaxEndurance, _maxEndurance);
        }

        public int GetTitleValue(TitleValue titleValue)
        {
            return DataHandler.GetUnsafeValueInt(customDataTitle, titleValue.ToString());
        }
        
        private int GetData(UserData.UserDataType dataKey)
        {
            var value = _cacheUserInfo.data.GetUserData(dataKey);
            if (value == null) return 0;
            return int.Parse(value);
        }

        private int GetUserStatistic(UserPayload.Statistics statistics)
        {
            return _cacheUserInfo.payload.GetStatisticValue(statistics);
        }

        private Dictionary<string, string> GetCustomData(CacheTitleData.TitleDataKey statsKey)
        {
            string customDataString = _cacheTitleData.GetTitleData(statsKey);
            var customDataStats = DataHandler.Deserialize(customDataString);
            return customDataStats;
        }
        
        private string[] GetStartWeapon()
        {
            var startWeaponSubclass = _cacheItemInfo
                .CreateItemList(ItemInfo.Catalog.Weapons, ItemInfo.Class.Firearm, startWeapon);

            var startWeaponItem = startWeaponSubclass[Random.Range(0, startWeaponSubclass.Count - 1)];
            return new string[] { startWeaponItem.info.itemName.ToString() };
        }

        private async UniTask<string[]> GetSessionItems()
        {
            var result = await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.GetSessionItems);

            if (result == null) return null;
            if (result.FunctionResult == null) return null;

            return PlayFabSimpleJson.DeserializeObject<string[]>(result.FunctionResult.ToString());
        }
    }
}
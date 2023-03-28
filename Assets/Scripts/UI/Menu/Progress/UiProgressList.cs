using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiProgressList : UiFactory
    {
        public Transform focus;
        public UiProgressNewLevel UiProgressNewLevel;
        public GameObject unlockContentButton;
        public UiProgressItemShow UiProgressItemShow;

        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private CacheTitleData _cacheTitleData;
        [Inject] private HandlerLoading _handlerLoading;

        public enum SeasonName
        {
            Anarchy
        }

        private void Start()
        {
            CreateProgressList(_cacheUserSettings.pickedSeason);
        }

        private List<UiProgressBundleSlot> lockedBundles = new();

        private async void CreateProgressList(string seasonName)
        {
            Debug.Log("CreateProgressList: " + seasonName);
            
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Updating);

            var seasonItems = GetSeasonItems();

            if (seasonItems == null)
            {
                Debug.LogError("Items is null for " + seasonName);
                return;
            }

            var userProgress = GetProgress(seasonName);
            var userPass = GetBattlePassState();
            var seasonLevels = GetSeasonLevels();
            
            Debug.Log("userProgress: " + userProgress + 
                      " userPass: " + userPass + " seasonLevels: " + seasonLevels);

            for (int i = 1; i < seasonLevels + 1; i++)
            {
                var bundleItems = seasonItems
                    .FindAll(item => item.info.GetRequiredLevel() == i);

                if (bundleItems == null || bundleItems.Count != 2) continue;

                var bundleSlotInstance = await CreateSlot(SlotName.ProgressBundleSlot);

                if (bundleSlotInstance.TryGetComponent(out UiProgressBundleSlot bundleSlot))
                {
                    var isLocked = i > userProgress;

                    bundleSlot.SetBundleItems(bundleItems);
                    bundleSlot.UpdateBundleItems(userPass, isLocked);
                    bundleSlot.SetBundleLevel(i, seasonLevels);
                    bundleSlot.SetBundleItemDemo(UiProgressItemShow);

                    if (i == userProgress) bundleSlot.SetFocus(focus);
                    if (isLocked) lockedBundles.Add(bundleSlot);
                }
            }

            _handlerLoading.OpenLoadingScreen(false);
            
            unlockContentButton.SetActive(!userPass);

            UiProgressNewLevel.CheckNewLevel(lockedBundles);
        }

        private List<Item> GetSeasonItems()
        {
            var seasonItemNames = GetSeasonBundle().info
                .GetCatalogItem().Bundle.BundledItems.ToArray();

            return GetItemInfo().CreateItemList
                (seasonItemNames, ItemInfo.Catalog.Character);
        }

        private int GetSeasonLevels()
        {
            var value = _cacheTitleData
                .GetTitleData(CacheTitleData.TitleDataKey.AnarchySeasonLevels);

            if (value == null) return 0;

            return int.Parse(value);
        }

        private Item GetSeasonBundle()
        {
            return GetItemsFromSource(ItemSource.External, ItemInfo.Catalog.Character,
                ItemInfo.Class.Season, ItemInfo.Subclass.Anarchy)[0];
        }

        private bool GetBattlePassState()
        {
            var value = GetUserInfo().data.GetUserData(UserData.UserDataType.BattlePass);
            if (value == null) return false;
            return bool.Parse(value);
        }

        private int GetProgress(string season)
        {
            int progress = 0;

            if (season == SeasonName.Anarchy.ToString())
            {
                progress = GetUserInfo().payload
                    .GetStatisticValue(UserPayload.Statistics.AnarchySeasonPoints);
            }

            return progress;
        }
    }
}

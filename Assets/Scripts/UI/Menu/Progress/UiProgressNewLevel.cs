using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiProgressNewLevel : MonoBehaviour
    {
        public List<Button> blockButtons;
        public Transform focus;
        public UiProgressNewItem UiProgressNewItem;
        
        [Inject] private CacheUserInfo _cacheUserInfo;
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private HandlerLoading _handlerLoading;

        public async void CheckNewLevel(List<UiProgressBundleSlot> lockedBundles)
        {
            if (GetNewLevelPoints() == 0) return;
            
            _handlerLoading.OpenLoadingScreen(true, HandlerLoading.Text.Updating);

            Debug.Log("User has a new level points: " + GetNewLevelPoints());

            var lockedBundlesOrdered = lockedBundles
                .OrderBy(slot => slot.GetLevelNumber()).ToList();

            var battlePass = GetBattlePass();

            List<string> itemsToGrant = new List<string>();

            for (int i = 0; i < GetNewLevelPoints(); i++)
            {
                var bundleSlot = lockedBundlesOrdered[i];
                bundleSlot.UpdateBundleItems(battlePass, false);
                bundleSlot.SetFocus(focus);

                foreach (var item in bundleSlot.GetBundleItems())
                {
                    itemsToGrant.Add(item.info.itemName);

                    if (item.info.IsPremium() == battlePass)
                    {
                        _cacheUserInfo.inventory.SetGrantedItem(item);
                    }
                }
            }

            await GrantNewItems(itemsToGrant);
            
            _handlerLoading.OpenLoadingScreen(false);

            BlockBackButtons(true);
            
            await UpdateTargetStatistics();
            await _cacheUserInfo.payload.UpdatePayload();
            await _cacheUserInfo.inventory.Install(_cacheItemInfo);

            BlockBackButtons(false);
        }

        private bool GetBattlePass()
        {
            var value = _cacheUserInfo.data
                .GetUserData(UserData.UserDataType.BattlePass);
            if (value == null) return false;
            return bool.Parse(value);
        }
        
        private int GetNewLevelPoints()
        {
            return _cacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.NewLevelPoints);
        }

        private async UniTask GrantNewItems(List<string> itemsToGrant)
        {
            var args = new
            {
                Catalog = "Character", ItemsArray = itemsToGrant.ToArray()
            };

            var result = await PlayFabHandler
                .ExecuteCloudScript(PlayFabHandler.Function.GrantItemsArray, args);

            var resultItems = PlayFabSimpleJson
                .DeserializeObject<ItemInstance[]>(result.FunctionResult.ToString());

            UiProgressNewItem.LoadNewItems(resultItems.ToList());
        }

        private async UniTask UpdateTargetStatistics()
        {
            if (GetNewLevelPoints() == 0) return;

            List<StatisticUpdate> statistics = new();

            statistics.Add(PlayFabHandler
                .CreateStatistic(UserPayload.Statistics.Level, GetNewLevelPoints()));

            statistics.Add(PlayFabHandler
                .CreateStatistic(UserPayload.Statistics.AnarchySeasonPoints, GetNewLevelPoints()));

            statistics.Add(PlayFabHandler
                .CreateStatistic(UserPayload.Statistics.NewLevelPoints, 0));

            await PlayFabHandler.ReportStatisticToServer(statistics);
        }
        
        private void BlockBackButtons(bool state)
        {
            foreach (var button in blockButtons)
            {
                button.interactable = !state;
            }
        }
    }
}
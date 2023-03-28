using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(UiUserInfoHandle))]
public class UiRatingList : UiFactory
{
    public Transform catalogButtonsLayout;
    public UiUserSlotMine UiUserSlotMine;
    
    private UiUserInfoHandle _uiUserInfoHandle;
    private List<UiRatingButton> _uiRatingButtons = new();

    [Inject]
    private HandlerLoading _handlerLoading;

    [Inject]
    private CacheRatingList _cacheRatingList;
    
    private void Awake()
    {
        _uiRatingButtons = catalogButtonsLayout.GetComponentsInChildren<UiRatingButton>().ToList();
        _uiUserInfoHandle = GetComponent<UiUserInfoHandle>();
    }

    public async void UpdateRatingTable(UserPayload.Statistics statistics)
    {
        ClearFactoryContainer();

        ActiveCatalogButtonFocus(statistics);

        _handlerLoading.OpenLoadingPopup(true, HandlerLoading.Text.Updating);

        UiUserSlotMine.SetStatistics(statistics);

        var leaderboardEntries = await _cacheRatingList
            .GetLeaderboardEntries(statistics);

        if (leaderboardEntries == null)
        {
            _handlerLoading.OpenLoadingPopup(false);
            return;
        }

        foreach(var entry in leaderboardEntries)
        {
            var ratingSlotInstance = await CreateSlot(SlotName.UserSlotMenu);

            if(ratingSlotInstance.TryGetComponent(out UiUserSlot userInfo))
            {
                userInfo.SetPlayFabId(entry.PlayFabId);
                userInfo.SetPlayFabProfile(entry.Profile);
                userInfo.SetInfoHandle(_uiUserInfoHandle);
                userInfo.SetUserStatus(UiFriends.Status.External);

                if (UiUserSlotMine.GetMinePlayFabId() == entry.PlayFabId)
                {
                    userInfo.BlockUserButton();
                }
            }
            
            if(ratingSlotInstance.TryGetComponent(out UiUserSlotPlayFab playFabInfo))
            {
                playFabInfo.SetProfileInfo(entry.Profile);
                playFabInfo.SetStatus(entry.StatValue.ToString());
            }
        }

        _handlerLoading.OpenLoadingPopup(false);
    }

    public void ActiveCatalogButtonFocus(UserPayload.Statistics statistics)
    {
        foreach (var catalogButton in _uiRatingButtons)
        {
            catalogButton.SetFocus(catalogButton.currentFilter == statistics);
        }
    }
    
    [ContextMenu("AddTestStat")]
    public async void AddTestStat()
    {
        var result = await PlayFabHandler
            .ReportStatisticToServer(UserPayload.Statistics.Frags, 10);
        
        if(result == null) return;
        
        Debug.Log("Test stat is added successful");
    }
}

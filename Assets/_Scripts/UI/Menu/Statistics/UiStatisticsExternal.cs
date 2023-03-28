using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using Zenject;

public class UiStatisticsExternal : UiStatistics
{
    public GetPlayerCombinedInfoResultPayload externalPayload;

    [Inject] private Unit _unit;

    [Inject] private UIElementContainer _uiElementContainer;

    [Inject] private HandlerLoading _handlerLoading;

    public void Start()
    {
        SetExternalUserPayload();
    }

    public async void SetExternalUserPayload()
    {
        var externalUserId=  _uiElementContainer.GetClipboardString
            (UIElementContainer.ClipboardStringType.FriendId);
        
        if(string.IsNullOrEmpty(externalUserId)) return;
        
        _handlerLoading.OpenLoadingScreen(true);
        
        var result = await PlayFabHandler
            .GetUserPayload(externalUserId, PlayFabHandler.ExternalPayloadParamerets());
        
        if (result.InfoResultPayload == null)
        {
            Debug.Log("User Data is null for " + externalUserId);
            _handlerLoading.OpenLoadingScreen(false);
            return;
        }
        
        if (result.InfoResultPayload.UserData.Count == 0)
        {
            Debug.Log("User Data is null for " + externalUserId);
            _handlerLoading.OpenLoadingScreen(false);
            return;
        }
        
        externalPayload = result.InfoResultPayload;
        SetPayload(externalPayload);
        
        await BuildExternalUserUnit();
        
        _handlerLoading.OpenLoadingScreen(false);
    }

    private async UniTask BuildExternalUserUnit()
    {
        var unitSkin = GetUserData(UserData.UserDataType.UnitSkin);
        
        var itemNamesString = GetUserData(UserData.UserDataType.Items);
        var itemNamesList = DataHandler.SplitString(itemNamesString).ToArray();
        
        await _unit.Builder.BuildUnit
            (itemNamesList, unitSkin, ItemInfo.Class.Firearm, ItemInfo.Subclass.Autogun, false);
    }
    
    public string GetUserData(UserData.UserDataType type)
    {
        externalPayload.UserData.TryGetValue(type.ToString(), out var value);
        
        if (value == null)
        {
            Debug.LogError("Failed to find user data record: " + type); return null;
        }
        
        return value.Value;
    }

    public void OnDestroy()
    {
        _unit.Builder.ReturtMineSetup();
    }
}

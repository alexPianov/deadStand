using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using Playstel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UiUserSlotPlayFab : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI userName;
    public TextMeshProUGUI userLevel;
    public TextMeshProUGUI userTextStatus;
    public Image userCountryIcon;
    
    private Button _button;
    private ObscuredString _userPlayFabId;
    private string lastSessionText;

    [Inject] private CacheSprites _cacheSprites;

    public void SetProfileInfo(PlayerProfileModel profileModel)
    {
        SetUsername(profileModel.DisplayName);
        SetProfileLevel(profileModel.Statistics);
        SetProfileCountryCode(profileModel.Locations);
    }

    private void SetUsername(string nickName)
    {
        userName.text = nickName;
    }

    private void SetProfileLevel(List<StatisticModel> statistics)
    {
        var levelKey = UserPayload.Statistics.Level.ToString();

        if (statistics == null)
        {
            Debug.Log("Statistics is null for " + userName.text);
            return;
        }
        
        var value = statistics
            .Find(info=> info.Name == levelKey).Value;
        
        userLevel.text = value.ToString();
    }
    
    private void SetProfileCountryCode(List<LocationModel> locations)
    {
        if(locations[0] == null)
        {
            userCountryIcon.gameObject.SetActive(false);
            Debug.Log("Failed to load locations"); return;
        }

        string countryCode = locations[0].CountryCode.ToString().ToLower();

        Sprite countrySprite = _cacheSprites.GetSpriteFromAtlas
            (countryCode, ItemInfo.Catalog.Setup);

        userCountryIcon.gameObject.SetActive(countrySprite);
        if (countrySprite) userCountryIcon.sprite = countrySprite;
    }
    
    public void SetStatus(UiFriends.Status status)
    {
        if (status == UiFriends.Status.Requested) 
            SetStatus("Reviewing your request");
        
        if (status == UiFriends.Status.External) 
            SetStatus("");
        
        if (status == UiFriends.Status.Pending) 
            SetStatus("Waiting to be added");
    }

    public void SetStatus(string status)
    {
        userTextStatus.text = status;
    }

}

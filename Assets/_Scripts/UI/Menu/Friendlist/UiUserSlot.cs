using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UiUserSlot : MonoBehaviour
{
    public UiFriends.Status Status;
    public ObscuredString PlayFabId;
    public PlayerProfileModel PlayFabProfile;
    public Photon.Realtime.FriendInfo PhotonFriendInfo;
    
    private UiUserInfoHandle _uiUserInfoHandle;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenPickedUserInfo);
    }

    public Photon.Realtime.FriendInfo GetPhotonFriendInfo()
    {
        return PhotonFriendInfo;
    }
    
    public void SetInfoHandle(UiUserInfoHandle infoHandle)
    {
        _uiUserInfoHandle = infoHandle;
    }
    
    public void SetUserStatus(UiFriends.Status status)
    {
        Status = status;
    }

    public void SetPlayFabId(ObscuredString playFabId)
    {
        PlayFabId = playFabId;
    }

    public void BlockUserButton()
    {
        GetComponent<Button>().interactable = false;
    }

    public void SetPlayFabProfile(PlayerProfileModel profileModel)
    {
        PlayFabProfile = profileModel;
    }

    public void SetPhotonData(Photon.Realtime.FriendInfo friendInfo)
    {
        PhotonFriendInfo = friendInfo;

        if (friendInfo == null)
        {
            Debug.Log("Friendinfo is null");
        }
    }
    
    public string GetProfileLastLoginDate()
    {
        var date = PlayFabProfile.LastLogin.Value.Date.ToShortDateString();
        return string.Format("Last seen {0}", date);
    }

    private void OpenPickedUserInfo()
    {
        _uiUserInfoHandle.OpenUserInfo(this);
    }
}

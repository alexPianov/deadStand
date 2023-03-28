using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiUserSlotPhoton : MonoBehaviour
{
    public Image userOnlineStatus;
    public TextMeshProUGUI userTextStatus;

    public void SetRealtimeInfo(FriendInfo friendInfo)
    {
        if (friendInfo == null)
        {
            Debug.Log("Failed to find photon friend info");
            return;
        }
        OnlineMark(friendInfo.IsOnline);
    }
    
    public void OnlineMark(bool state)
    {
        userOnlineStatus.enabled = state;
    }

    public void SetStatus(FriendInfo friendInfo)
    {
        if (friendInfo == null) return;
        if (friendInfo.IsOnline) userTextStatus.text = "Online";
        if (friendInfo.IsInRoom) userTextStatus.text = "Play In Room";
        if (!friendInfo.IsOnline) userTextStatus.text = LastLoginDate();
    }

    private string LastLoginDate()
    {
        return GetComponent<UiUserSlot>().GetProfileLastLoginDate();
    }
}

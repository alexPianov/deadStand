using Playstel;
using UnityEngine;
using Zenject;

public class UiStatisticsMine : UiStatistics
{
    [Inject] private CacheUserInfo _cacheUserInfo;

    private void Start()
    {
        var payload = _cacheUserInfo.payload.GetPlayFabPayload();
        if(payload == null) { Debug.LogError("Failed to find user payload"); }
        SetPayload(payload);
    }
}

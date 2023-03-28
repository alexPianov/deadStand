using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using Playstel;
using TMPro;
using UnityEngine;

public class UiStatistics : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI nickname;
    public Transform statisticsLayout;

    private List<UiStatisticsDisplay> _statisticsDisplays = new();

    private void Awake()
    {
        _statisticsDisplays = statisticsLayout.GetComponentsInChildren<UiStatisticsDisplay>().ToList();
    }

    private GetPlayerCombinedInfoResultPayload _payload;
    public void SetPayload(GetPlayerCombinedInfoResultPayload payload)
    {
        _payload = payload;

        foreach (var statistic in _statisticsDisplays)
        {
            statistic.SetStatisticValue(_payload);
        }
        
        if (nickname) nickname.text = _payload.PlayerProfile.DisplayName; 
    }
}

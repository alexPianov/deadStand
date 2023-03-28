using PlayFab.ClientModels;
using Playstel;
using TMPro;
using UnityEngine;

public class UiStatisticsDisplay : MonoBehaviour
{
    public UserPayload.Statistics currentStatistic;
    public TextMeshProUGUI value;

    public void SetStatisticValue(GetPlayerCombinedInfoResultPayload payload)
    {
        var stat = payload.PlayerStatistics
            .Find(info => info.StatisticName == currentStatistic.ToString());

        if(stat == null)
        {
            Debug.LogError("Failed to find statistic string: " + currentStatistic); return;
        }

        if (currentStatistic == UserPayload.Statistics.GameTime)
        {
            value.text = GetTime(stat);
        }
        else
        {
            value.text = stat.Value.ToString();
        }
    }

    private string GetTime(StatisticValue stat)
    {
        var hours = Mathf.FloorToInt(stat.Value / 60);
        var minutes = stat.Value - hours * 60;
        return string.Format("{0} h {1} min", hours, minutes);
    }
}

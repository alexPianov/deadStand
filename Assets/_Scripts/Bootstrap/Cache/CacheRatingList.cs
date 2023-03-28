using System.Collections.Generic;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playstel
{
    public class CacheRatingList : MonoBehaviour
    {
        private Dictionary<UserPayload.Statistics, GetLeaderboardResult> resultCache = new();

        public async Task<List<PlayerLeaderboardEntry>> GetLeaderboardEntries
            (UserPayload.Statistics statistics)
        {
            if (resultCache.ContainsKey(statistics))
            {
                resultCache.TryGetValue(statistics, out var value);
                return value.Leaderboard;
            }
            
            var result = await PlayFabHandler.GetLeaderboardData(statistics);
            
            if (result == null) return null;
            
            resultCache.Add(statistics, result);
            
            return result.Leaderboard;
        }

        public void ClearLeaderboardCache()
        {
            resultCache.Clear();
        }
    }
}

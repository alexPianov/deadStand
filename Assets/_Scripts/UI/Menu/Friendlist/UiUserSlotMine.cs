using Zenject;

namespace Playstel
{
    public class UiUserSlotMine : UiUserSlotPlayFab
    {
        [Inject]
        private CacheUserInfo _cacheUserInfo;
        
        void Start()
        {
            SetProfileInfo(_cacheUserInfo.payload.GetPlayFabPayload().PlayerProfile);
        }

        public void SetStatistics(UserPayload.Statistics statistics)
        {
            var mineResult = _cacheUserInfo.payload.GetStatisticValue(statistics);
            SetStatus(mineResult.ToString());
        }

        public string GetMinePlayFabId()
        {
            return _cacheUserInfo.payload.GetPlayFabId();
        }
    }
}

using System;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class LevelUpCheck : MonoBehaviour
    {
        [Inject] private CacheUserInfo _cacheUserInfo;

        public PlayerBootMenu PlayerBootMenu;

        private void Start()
        {
            var levelPoints = _cacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.NewLevelPoints);
            
            PlayerBootMenu.disableLoadingScreen = levelPoints > 0;

            if (levelPoints > 0)
            {
                PlayerBootMenu.SpawnUnitInMenu(UIElementLoad.Elements.NewLevel);
            }
            else
            {
                PlayerBootMenu.SpawnUnitInMenu(UIElementLoad.Elements.MainMenu);
            }
        }
    }
}
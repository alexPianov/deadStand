using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UIElementLoad : MonoBehaviour
    {
        public Elements openingElement;

        [Inject]
        private LocationInstaller _installer;
        
        [Inject]
        private HandlerLoading _handlerLoading;

        public enum Elements 
        {
            MainMenu, Customize, CreateCharacter, Friends, Leaderboards, Shop, Progress, 
            Settings, Roulette, Statistics, EditCharacter, LogIn, SignUp, Null, Inventory, 
            GUI, LevelCamera, AimThrow, Dialog, ShopWeapons, ShopSupport, Sell, Crate, 
            StatisticsExternalLeaderboard, StatisticsExternalFriendlist, PreviousPopup, NetworkError, RoomLeaderboard, 
            Navigator, Pause, PickSide, KillerScreen, RoundResult, NewLevel, RewardItem, 
            SeasonAdd, SeasonList, Unavailable, RoomLeaderboardFinish, InstallAttention, FirstLogin
        }
    
        public async UniTask<GameObject> LoadElement(Transform parent)
        {
            if (openingElement == Elements.Null) return null;
            
            _handlerLoading.OpenLoadingScreen(true);
            
            var elementName = openingElement.ToString();
            var element = await _installer.LoadElement<GameObject>(elementName, parent);

            _handlerLoading.OpenLoadingScreen(false);
            
            return element;
        }
    }
}
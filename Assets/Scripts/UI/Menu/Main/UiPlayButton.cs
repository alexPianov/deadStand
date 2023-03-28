using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(Button))]
    public class UiPlayButton : MonoBehaviour
    {
        [Inject] private ConnectRoom _connectRoom;
        [Inject] private Unit _unit;
        [Inject] private CacheRatingList _cacheRatingList;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private async void Start()
        {
            if(!_button) return;
            await ButtonActivation();
            _button.onClick.AddListener(Action);
        }

        private async UniTask ButtonActivation()
        {
            _button.interactable = false;
            await UniTask.WaitUntil(() => _unit);
            await UniTask.WaitUntil(() => _unit.Builder.IsBuild);
            if(!_button) return;
            _button.interactable = true;
        }

        private void Action()
        {
            _cacheRatingList.ClearLeaderboardCache();
            _connectRoom.ConnectPickedRoom();
        }
    }
}

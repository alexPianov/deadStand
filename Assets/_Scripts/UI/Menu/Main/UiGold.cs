using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel.UI
{
    public class UiGold : MonoBehaviour
    {
        public TextMeshProUGUI gold;

        [Inject] private CacheUserInfo _cacheUserInfo;

        private void Start()
        {
            gold.text = _cacheUserInfo.payload.GetPlayerMainCurrency().ToString();
        }
    }
}

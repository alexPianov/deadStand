using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel.UI
{
    public class UiName : MonoBehaviour
    {
        public TextMeshProUGUI unitNickname;
        public UiCharacterNickname CharacterNickname;

        [Inject] private CacheUserInfo _cacheUserInfo;

        public async void Start()
        {
            var displayName = _cacheUserInfo.payload.GetTitleDisplayName();
            
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = await CreateCharacterNameSceen();
            }
            
            unitNickname.text = displayName;
        }

        private async Task<string> CreateCharacterNameSceen()
        {
            string displayName;
            CharacterNickname.ActivePanel(true);
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(CharacterNickname.characterName));

            displayName = CharacterNickname.characterName;

            _cacheUserInfo.payload.GetPlayFabPayload()
                .AccountInfo.TitleInfo.DisplayName = displayName;
            return displayName;
        }
    }
}

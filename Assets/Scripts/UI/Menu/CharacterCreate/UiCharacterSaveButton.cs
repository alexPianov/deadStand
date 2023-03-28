using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class UiCharacterSaveButton : MonoBehaviour
    {
        [Header("Scene")]
        public SceneHandler.Scenes nextScene = SceneHandler.Scenes.Menu;
        
        [Header("Master")]
        public UiCharacterSaveMaster _characterSaveMaster;

        [Inject] private HandlerLoading _handlerLoading;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Save);
        }

        public async void Save()
        {
            if (!_characterSaveMaster)
            {
                Debug.LogError("Failed to find character save master");
                return;
            }
            
            await _characterSaveMaster.SaveCharacter();
            
            _handlerLoading.OpenBlackScreen(true);
            SceneHandler.LoadAsync(nextScene);
        }
    }
}

using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UIDialogHandle : MonoBehaviour
    {
        public TextMeshProUGUI characterName;
        
        [Inject]
        private Unit _unit;

        [Inject] 
        private HandlerLoading _handlerLoading;

        private NpcCharacterDialog _npcDialog;
        private UIDialogChat _dialogChat;
        private UIDialogButtonGroup _dialogButtons;

        //private GameObject _dialogCamera;

        private void Awake()
        {
            _dialogChat = GetComponent<UIDialogChat>();
            _dialogButtons = GetComponent<UIDialogButtonGroup>();
        }

        public async void Start()
        {
            _handlerLoading.OpenLoadingScreen(true);

            _unit.Aim.Aiming(false);
            
            await UniTask.WaitUntil(() => _unit.Interaction.DialogNpc);

            _npcDialog = _unit.Interaction.DialogNpc;

            _npcDialog.npcCharacterCamera.ActiveCamera(true);
            _unit.Camera.GetCameraObserver().ActiveCamera(false);
            //_dialogCamera = await _npcDialog.npcCharacterCamera.CreateCamera();

            _dialogChat.SetCharacterDialog(_npcDialog);
            _dialogChat.LoadPhrases(NpcCharacterProfile.Type.Info);
            
            _dialogButtons.Initialize(_npcDialog);
            
            _unit.Renderer.Active(false);
            _handlerLoading.OpenLoadingScreen(false);
            characterName.text = _npcDialog.npcCharacter.GetCharacterName();
        }
        
        public void ExitDialog()
        {
            _npcDialog.npcCharacterCamera.ActiveCamera(false);
            _unit.Camera.GetCameraObserver().ActiveCamera(true);
            _unit.Renderer.Active(true);
            _handlerLoading.OpenLoadingScreen(false);
        }

        public void OnDestroy()
        {
            ExitDialog();
        }
    }
}


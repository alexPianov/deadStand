using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Playstel
{
    public class RoomSceneEnter : MonoBehaviourPunCallbacks
    {
        public UIElementLoadOnBoot LoadUiOnBoot;
        //public RoomStartCutsceneLoader CutsceneLoader;

        [Inject] private HandlerLoading _handlerLoading;
        [Inject] private CacheAudio _cacheAudio;

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
            SceneManager.sceneLoaded += OnSceneFinishLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneFinishLoaded;
        }

        public async void OnSceneFinishLoaded(Scene scene, LoadSceneMode mode)
        {
            _handlerLoading.SetLoadingText(null);
            _handlerLoading.ActiveLoadingText(false);
            _handlerLoading.OpenBlackScreen(true);

            await UniTask.Delay(200);

            // if (CutsceneLoader)
            // {
            //     CutsceneLoader.Cutscene();
            //     return;
            // }
            
            ShowPickSideScreen();
        }
        
        public async void ShowPickSideScreen()
        {
            await LoadUiOnBoot.Load(UIElementLoad.Elements.PickSide, UIElementContainer.Type.Screen, true);
            
            await _cacheAudio.PlayMusic(CacheAudio.Music.Lobby);
            
            await UniTask.Delay(800);
            
            DisableLoadingSceen();
            
            _handlerLoading.OpenBlackScreen(false);
        }

        public void DisableLoadingSceen()
        {
            _handlerLoading.OpenLoadingScreen(false);
        }
    }
}

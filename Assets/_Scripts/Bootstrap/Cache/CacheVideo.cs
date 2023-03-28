using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;

namespace Playstel
{
    public class CacheVideo : MonoBehaviour
    {
        public GameObject closeButton;
        public GameObject blockScreen;
        public GameObject pauseIcon;
        public VideoPlayer VideoPlayer;
        public Camera Camera;

        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private HandlerPulse _handlerPulse;
        [Inject] private HandlerLoading _handlerLoading;

        private bool _enableCloseButton;

        public void InstallStartVideo(string currentSeason)
        {
            Debug.Log("Install Start Video: Video_" + currentSeason );
            Addressables.DownloadDependenciesAsync("Video_" + currentSeason);
        }

        public async UniTask PlayVideo(string video, bool enableCloseButton = true)
        {
            _handlerLoading.OpenLoadingScreen(true);

            var clipName = "Video_" + video;
            
            Debug.Log("Play Clip: " + clipName);
            
            var clip = await AddressablesHandler.Load<VideoClip>(clipName);

            if (clip == null)
            {
                Debug.LogError("Failed to find video: " + clipName);
                _handlerPulse.OpenTextNote("Failed to find video");
                _handlerLoading.OpenLoadingScreen(false);
                return;
            }

            _enableCloseButton = enableCloseButton;

            Debug.Log("PrepareClip: " + clip.name);
            PrepareClip(clip);
            
            await UniTask.WaitUntil(() => !VideoPlayer.clip);
            Debug.Log("Clip is finish");
        }

        private void PrepareClip(VideoClip videoClip)
        {
            VideoPlayer.clip = videoClip;
            VideoPlayer.Prepare();
            VideoPlayer.prepareCompleted += ReadyToPlay;
        }

        private void LoopPointReached(VideoPlayer vp)
        {
            StopVideo();
        }
        
        private void ReadyToPlay(VideoPlayer vp)
        {
            Debug.Log("ReadyToPlay");
            
            Debug.Log("closeButton: " + _enableCloseButton);
            closeButton.SetActive(_enableCloseButton);
            
            pauseIcon.SetActive(false);
            blockScreen.SetActive(true);

            _cacheUserSettings.DisableAudioMaster(true);
            Camera.enabled = true;
            
            _handlerLoading.OpenLoadingScreen(false);
            _handlerLoading.ActiveLoadingText(false);
            
            VideoPlayer.Play();
            
            VideoPlayer.loopPointReached += LoopPointReached;
        }

        [ContextMenu("Stop Video")]
        public void StopVideo()
        {
            Debug.Log("StopVideo");
            
            _cacheUserSettings.DisableAudioMaster(false);
            _handlerLoading.ActiveLoadingText(true);

            blockScreen.SetActive(false);
            closeButton.SetActive(false);
            pauseIcon.SetActive(false);

            Camera.enabled = false;
            VideoPlayer.clip = null;
            VideoPlayer.Stop();
        }

        public void PauseMode()
        {
            if (VideoPlayer.isPaused)
            {
                VideoPlayer.Play();
                pauseIcon.SetActive(false);
                return;
            }

            if (VideoPlayer.isPlaying)
            {
                VideoPlayer.Pause();
                pauseIcon.SetActive(true);
            }
        }
    }
}
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class RoomStartCutscene : MonoBehaviour
    {
        [Header("Camera")]
        public Animation cutsceneAnimation;

        [Header("Text")] 
        public UiTransparency textTransparency;
        public RoomStartCutscenePhrases CutscenePhrases;
        public TextMeshProUGUI cutscenePhrasesText;
        public int currentPhraseNumber;
        private List<string> cutscenePhrases = new();
        public SeasonSlot.Season seasonName = SeasonSlot.Season.Anarchy;
        
        [Header("Refs")]
        public RoomSceneEnter SceneEnter;
        public RoomStartCutsceneLoader CutsceneLoader;

        [Inject] private HandlerLoading _handlerLoading;

        [Inject] private CacheAudio _cacheAudio;

        public void StartCutscene()
        {
            cutscenePhrases = CutscenePhrases.GetCutscenePhrases();
            cutsceneAnimation.Play();
        }

        public void ShowText()
        {
            textTransparency.Enable(false);
        }
        
        public async void HideText()
        {
            textTransparency.Enable(true);
            await UniTask.Delay(300);
            cutscenePhrasesText.text = null;
        }
        
        public void OpenBlackScreen()
        {
            _handlerLoading.OpenBlackScreen(true);
        }

        public void CloseBlackScreen()
        {
            _handlerLoading.OpenBlackScreen(false);
        }

        public void PlaySplashSound()
        {
            _cacheAudio.Play(CacheAudio.MenuSound.OnSplash);
        }

        public void PlayCutsceneMusic()
        {
            _cacheAudio.PlayMusic(CacheAudio.Music.Cutscene, "_" + seasonName);
        }

        public void NextPhrase()
        {
            cutscenePhrasesText.text = cutscenePhrases[currentPhraseNumber];
            currentPhraseNumber++;
        }

        public void FinishCutscene()
        {
            CutsceneLoader.SaveCutsceneWatchEvent();
            SceneEnter.ShowPickSideScreen();
            Destroy(gameObject);
        }
    }
}
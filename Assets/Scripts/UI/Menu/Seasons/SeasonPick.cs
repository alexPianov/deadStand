using UnityEngine;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(UIElementLoadByType))]
    public class SeasonPick : MonoBehaviour
    {
        public Transform focus;
        public Canvas uiCanvas;
        
        [HideInInspector]
        [Inject] public CacheUserSettings CacheUserSettings;
        [Inject] private CacheVideo _cacheVideo;

        private void Awake()
        {
            var seasonSlots = GetComponentsInChildren<SeasonSlot>();

            foreach (var seasonSlot in seasonSlots)
            {
                seasonSlot.SetSave(this);
                seasonSlot.SetFocus(focus);
            }
        }

        public void PickSeason(SeasonSlot.Season season)
        {
            CacheUserSettings.SetSeason(season);
            
            GetComponent<UIElementLoadByType>()
                .Load(UIElementLoad.Elements.Progress, UIElementContainer.Type.Screen);
        }

        public async void ShowTrailer(SeasonSlot.Season season)
        {
            uiCanvas.enabled = false;
            
            Debug.Log("ShowTrailer | Canvas: " + uiCanvas.enabled);
            
            await _cacheVideo.PlayVideo(season.ToString());
            
            uiCanvas.enabled = true;
            
            Debug.Log("ShowTrailer | Canvas: " + uiCanvas.enabled);
        }
    }
}
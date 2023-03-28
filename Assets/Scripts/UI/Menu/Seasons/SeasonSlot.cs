using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class SeasonSlot : MonoBehaviour
    {
        public Season currentSeason;
        public TextMeshProUGUI seasonName;
        public TextMeshProUGUI seasonProgress;
        public Transform focusPlaceholder;
        private Transform _focus;
        
        [Header("Pick")]
        public Button button;
        public TextMeshProUGUI buttonText;
        public bool maxLevelReached;

        [Inject] private CacheTitleData _cacheTitleData;
        [Inject] private CacheUserInfo _cacheUserInfo;

        private SeasonPick _seasonPick;
        
        public enum Season
        {
            Anarchy
        }

        private ObscuredInt _userProgress;
        private int _maxProgress;

        public void InitialProgressValues()
        {
            seasonName.text = currentSeason.ToString();

            if (_cacheUserInfo == null)
            {
                Debug.Log("Cache User Info is null");
                return;
            }
            
            if (_cacheTitleData == null)
            {
                Debug.Log("Cache Title Info is null");
                return;
            }
            
            _userProgress = _cacheUserInfo.payload
                .GetStatisticValue(currentSeason + "SeasonPoints");
            
            _maxProgress = int.Parse(_cacheTitleData
                .GetTitleData(currentSeason + "SeasonLevels"));

            seasonProgress.text = "Progress: " + _userProgress + "/" + _maxProgress;
        }

        public void SetNewLevelMode()
        {
            button.onClick.AddListener(Pick);

            var levelPoints = _cacheUserInfo.payload
                .GetStatisticValue(UserPayload.Statistics.NewLevelPoints);
                
            buttonText.text = "Add " + levelPoints + " Points";

            if (_userProgress + levelPoints > _maxProgress)
            {
                button.interactable = false;
                maxLevelReached = true;
            }
        }

        public void SetCommonMode()
        {
            if (!_seasonPick)
            {
                Debug.LogError("Failed to find SeasonPicker");
                return;
            }
            
            if (_seasonPick.CacheUserSettings.pickedSeason == currentSeason.ToString())
            {
                button.onClick.AddListener(Trailer);
                buttonText.text = "Trailer";
                ActiveFocus();
            }
            else
            {
                button.onClick.AddListener(Pick);
                buttonText.text = "Pick";
            }
        }

        private void Pick()
        {
            _seasonPick.PickSeason(currentSeason);
            ActiveFocus();
        }

        private void Trailer()
        {
            _seasonPick.ShowTrailer(currentSeason);
        }

        public void SetSave(SeasonPick seasonPick)
        {
            _seasonPick = seasonPick;
        }

        public void SetFocus(Transform focus)
        {
            _focus = focus;
        }
        
        public void ActiveFocus()
        {
            _focus.SetParent(focusPlaceholder);
            _focus.localPosition = new Vector3(0, 0, 0);
        }
    }
}
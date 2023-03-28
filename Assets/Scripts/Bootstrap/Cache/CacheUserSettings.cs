using Assets.SimpleLocalization;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Playstel
{
    public class CacheUserSettings : MonoBehaviour
    {
        private readonly ObscuredString photonRealtimeAppId 
            = "792c9697-b675-40f5-a989-e814a62287f7";
        
        private readonly ObscuredString playFabTitleID = "6F24F";
        
        private readonly int targetFrameRate = 120;
        private readonly int vSyncCount = 0;

        public ObscuredString pickedLanguage;
        public ObscuredString pickedRegion;
        public ObscuredString pickedSeason;
        
        public ObscuredInt pickedRegionNumber;
        public ObscuredInt qualityLevel;
        
        public ObscuredFloat musicLevel;
        public ObscuredFloat soundLevel;
        
        public ObscuredBool PostProcessing;
        public ObscuredBool Blood;
        public ObscuredBool Vibration;
        public ObscuredBool PushAlarms;
        public ObscuredBool Shadows;
        public ObscuredBool CoreInstalled;

        [Header("Localization")]
        public bool setDefaultLanguage = true;
        public string defaultLanguage = "English";
        
        [Header("Audio")]
        public AudioMixer SoundMixer;
        public AudioMixer MusicMixer;

        [Header("Debugging")] 
        public StackTraceLogType StackTrace = StackTraceLogType.ScriptOnly;

        public async UniTask LoadSettings()
        {
            SetDebugStackTraces();
            SetPlayFabId();
            SetGraphicSetting();
            
            LoadInstallStatus();
            LoadLanguage();
            LoadSoundMixer();
            LoadMusicMixer();
            LoadQualityLevel();
            LoadShadows();
            LoadBlood();
            LoadRegion();
            LoadPostProcessing();
            LoadPushAlarms();
            LoadVibration();
            LoadSeason();
        }

        public string GetPhotonRealtimeAppId()
        {
            return photonRealtimeAppId;
        }
        
        public string GetPlayFabTitleId()
        {
            return playFabTitleID;
        }
        
        #region Set Values

        private void SetDebugStackTraces()
        {
            Application.stackTraceLogType = StackTrace;
        }

        private void SetPlayFabId()
        {
            PlayFabSettings.TitleId = playFabTitleID;
        }
        
        private void SetGraphicSetting()
        {
            QualitySettings.vSyncCount = vSyncCount;
            Application.targetFrameRate = targetFrameRate;
        }

        public void SetCoreInstall(bool state, bool saveInPrefs = true)
        {
            CoreInstalled = state;
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.StringKey.Install, 
                CoreInstalled.ToString());
        }
        
        public void SetSeason(SeasonSlot.Season currentSeason)
        {
            pickedSeason = currentSeason.ToString();
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Season, pickedSeason);
        }
        
        public void SetRegion(int region, bool saveInPrefs = true)
        {
            var regionString = KeyStore.GetRegion(region);
            SetRegion(regionString, saveInPrefs);
        }

        public void SetRegion(string region = null, bool saveInPrefs = true)
        {
            if (string.IsNullOrEmpty(region))
            {
                pickedRegionNumber = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Region);
                region = KeyStore.GetRegion(pickedRegionNumber);
            }

            pickedRegion = region;
            pickedRegionNumber = KeyStore.GetRegion(pickedRegion);
            
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = pickedRegion;
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Region, pickedRegionNumber);
        }

        public void SetLanguage(string language, bool saveInPrefs = true)
        {
            LocalizationManager.Read();
            
            if (string.IsNullOrEmpty(language))
            {
                if (setDefaultLanguage)
                {
                    language = defaultLanguage;
                }
                else
                {
                    language = Application.systemLanguage.ToString();
                }
            }

            pickedLanguage = language;
            LocalizationManager.Language = language;
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.StringKey.Language, language);
        }
        
        public void SetQualityLevel(float value, bool saveInPrefs = true)
        {
            var valueInt = Mathf.RoundToInt(value);
            QualitySettings.SetQualityLevel(valueInt);
            qualityLevel = valueInt;
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Quality, valueInt);
        }

        private const float _logCoeff = 30;
        public void SetMusicLevel(float value, bool saveInPrefs = true)
        {
            musicLevel = value;
            MusicMixer.SetFloat("Volume", Mathf.Log10(musicLevel * 0.01f) * _logCoeff);
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.FloatKey.MusicFx, value);
        }
        
        public void SetSoundLevel(float value, bool saveInPrefs = true)
        {
            soundLevel = value;
            SoundMixer.SetFloat("Volume", Mathf.Log10(soundLevel *  0.01f) * _logCoeff);
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.FloatKey.SoundFx, value);
        }

        public void SetShadows(int value, bool saveInPrefs = true)
        {
            Shadows = value == 0;
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Shadows, value);
        }

        private Volume _volume;
        public void SetPostProcessingVolume(Volume volume)
        {
            _volume = volume;
        }
        
        public void SetPostProcessing(int value, bool saveInPrefs = true)
        {
            PostProcessing = value == 0;
            
            _volume.enabled = PostProcessing;
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.PostProcessing, value);
        }

        public void SetBlood(int value, bool saveInPrefs = true)
        {
            Blood = value == 0;
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Blood, value);
        }

        public void SetVibrations(int value, bool saveInPrefs = true)
        {
            Vibration = value == 0;
            
            if (value == 1)
            {
            }
            else
            {
            }
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Vibration, value);
        }

        public void SetPushAlarms(int value, bool saveInPrefs = true)
        {
            PushAlarms = value == 0;
            
            if (value == 1)
            {
            }
            else
            {
            }
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.PushAlarm, value);
        }

        private const int minDb = -80;
        public bool disableAudio;
        public void DisableAudioMaster(bool state)
        {
            disableAudio = state;

            if (disableAudio)
            {
                SoundMixer.SetFloat("Volume", minDb);
                MusicMixer.SetFloat("Volume", minDb);
            }
            else
            {
                LoadSoundMixer();
                LoadMusicMixer();
            }
        }

        #endregion

        #region Load Values

        public void LoadSeason()
        {
            pickedSeason = PreferenceHandler
                .GetValue(PreferenceHandler.StringKey.Season);

            if (string.IsNullOrEmpty(pickedSeason))
            {
                SetSeason(SeasonSlot.Season.Anarchy);
            }
        }
        
        public void LoadRegion()
        {
            pickedRegionNumber = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Region);
            SetRegion(pickedRegionNumber, false);
        }

        public void LoadInstallStatus()
        {
            var value = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Install);
            if(string.IsNullOrEmpty(value)) { SetCoreInstall(false); return; }
            var state = bool.Parse(value);
            SetCoreInstall(state, false);
        }
        
        public void LoadLanguage()
        {
            var language = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Language);
            SetLanguage(language, false);
        }

        public void LoadSoundMixer()
        {
            var sound = PreferenceHandler.GetValue(PreferenceHandler.FloatKey.SoundFx);
            if (sound == 0) sound = 80;
            SetSoundLevel(sound, false);
        }

        public void LoadMusicMixer()
        {
            var music = PreferenceHandler.GetValue(PreferenceHandler.FloatKey.MusicFx);
            if (music == 0) music = 60;
            SetMusicLevel(music, false);
        }

        public void LoadQualityLevel()
        {
            var quality = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Quality);
            SetQualityLevel(quality, false);
        }

        public void LoadShadows()
        {
            var shadows = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Shadows);
            SetShadows(shadows, false);
        }
        
        public void LoadBlood()
        {
            var blood = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Blood);
            SetBlood(blood, false);
        }
        
        public void LoadPostProcessing()
        {
            var postProcessingValue = PreferenceHandler.GetValue(PreferenceHandler.IntKey.PostProcessing);
            SetPostProcessing(postProcessingValue, false);
        }

        public void LoadPushAlarms()
        {
            var pushAlarms = PreferenceHandler.GetValue(PreferenceHandler.IntKey.PushAlarm);
            SetPushAlarms(pushAlarms, false);
        }
        
        public void LoadVibration()
        {
            var vibration = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Vibration);
            SetVibrations(vibration, false);
        }
        

        #endregion
    }
}

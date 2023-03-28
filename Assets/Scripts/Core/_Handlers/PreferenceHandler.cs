using Photon.Pun;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public static class PreferenceHandler
    {
        public enum IntKey
        {
            PushAlarm, Vibration, PostProcessing, Shadows, Region, Blood, Quality
        }
        
        public enum FloatKey
        {
            SoundFx, MusicFx
        }

        public enum StringKey
        {
            Language, Nickname, Password, Email, Season, Install
        }
        
        public static int GetValue(IntKey key)
        {
            return PlayerPrefs.GetInt(key.ToString());
        }
        
        public static float GetValue(FloatKey key)
        {
            return PlayerPrefs.GetFloat(key.ToString());
        }
        
        public static string GetValue(StringKey key)
        {
            return PlayerPrefs.GetString(key.ToString());
        }

        public static void SetValue(IntKey key, int value)
        {
            PlayerPrefs.SetInt(key.ToString(), value);
            PlayerPrefs.Save();
        }
        
        public static void SetValue(FloatKey key, float value)
        {
            var rounded = (float)Math.Round(value, 2);
            PlayerPrefs.SetFloat(key.ToString(), rounded);
            PlayerPrefs.Save();
        }

        public static void SetValue(StringKey key, string value)
        {
            PlayerPrefs.SetString(key.ToString(), value);
            PlayerPrefs.Save();
        }
    }
}

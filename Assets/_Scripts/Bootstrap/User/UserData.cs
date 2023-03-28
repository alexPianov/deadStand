using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "User/Data")]
    public class UserData : ScriptableObject
    {
        private UserPayload _userPayload;
        
        public Dictionary<ObscuredString, ObscuredString> userDataSafe;
        public enum UserDataType
        {
            Newly, UnitSkin, Items, Bag, Lives, Currency, BattlePass, Cutscenes
        }

        public ObscuredString GetUserData(UserDataType type)
        {
            userDataSafe.TryGetValue(type.ToString(), out ObscuredString value);
            
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                Debug.Log(type + " value is null or empty");
                return null;
            }

            return value;
        }

        public void SetNewlyStatusLocally(bool state)
        {
            userDataSafe.Remove(UserDataType.Newly.ToString());
            userDataSafe.Add(UserDataType.Newly.ToString(), state.ToString());
        }

        public void SetUnitSkin(string skinName)
        {
            userDataSafe.Remove(UserDataType.UnitSkin.ToString());
            userDataSafe.Add(UserDataType.UnitSkin.ToString(), skinName);
        }
        
        public void ResetCutsceneStatusForSeason(string seasonName)
        {
            userDataSafe.TryGetValue(UserDataType.Cutscenes.ToString(), out var cutscenesInfo);
            var cutscenes = DataHandler.Deserialize(cutscenesInfo);
            
            cutscenes.Remove(seasonName);
            cutscenes.Add(seasonName, "false");
            var cutscenesString = DataHandler.DictionaryToString(cutscenes);
            
            userDataSafe.Remove(UserDataType.Cutscenes.ToString());
            userDataSafe.Add(UserDataType.Cutscenes.ToString(), cutscenesString);
        }

        public void UpdateUserDataRecord(Dictionary<string, UserDataRecord> userData)
        {
            userDataSafe = DataHandler.ConvertToSafeData(userData);
        }
    }
}

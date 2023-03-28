using System;
using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playstel
{
    public static class DataHandler
    {
        public static Dictionary<ObscuredString, ObscuredString> ConvertToSafeData(Dictionary<string, string> unsafeData)
        {
            var safeData = new Dictionary<ObscuredString, ObscuredString>();

            foreach (var data in unsafeData)
                safeData.Add(data.Key, data.Value);

            return safeData;
        }

        public static Dictionary<ObscuredString, ObscuredInt> ConvertToSafeData(Dictionary<string, uint> unsafeData)
        {
            var safeData = new Dictionary<ObscuredString, ObscuredInt>();

            foreach (var data in unsafeData)
                safeData.Add(data.Key, (int)data.Value);

            return safeData;
        }

        public static List<ObscuredString> ConvertToSafeData(List<string> unsafeData)
        {
            var safeData = new List<ObscuredString>();

            for (int i = 0; i < unsafeData.Count; i++)
            {
                safeData.Add(unsafeData[i]);
            }

            return safeData;
        }

        public static Dictionary<ObscuredString, ObscuredString> ConvertToSafeData(Dictionary<string, UserDataRecord> unsafeData)
        {
            var safeData = new Dictionary<ObscuredString, ObscuredString>();

            foreach (var data in unsafeData)
            {
                safeData.Add(data.Key, data.Value.Value);
            }

            return safeData;
        }

        public static Dictionary<string, string> ConvertToUnsafeData(Dictionary<ObscuredString, ObscuredString> safeData)
        {
            var unsafeData = new Dictionary<string, string>();

            foreach (var data in safeData)
                unsafeData.Add(data.Key, data.Value);

            return unsafeData;
        }


        public static ObscuredString GetSafeValue(Dictionary<ObscuredString, ObscuredString> dictionary, string key)
        {
            if (dictionary.Count == 0)
            {
                Debug.LogError("Dictionary data is null");
                return null;
            }

            if (!dictionary.ContainsKey(key))
            {
                Debug.Log("No " + key + " in dictionary data");
                return null;
            }

            dictionary.TryGetValue(key, out ObscuredString result);

            return result;
        }

        public static string GetUnsafeValue(Dictionary<string, string> dictionary, string key)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                Debug.LogError("Dictionary data is null");
                return null;
            }

            if (!dictionary.ContainsKey(key))
            {
                return null;
            }

            dictionary.TryGetValue(key, out string result);

            return result;
        }

        public static int GetUnsafeValueInt(Dictionary<string, string> customData, string statName)
        {
            var value = GetUnsafeValue(customData, statName);
            if (string.IsNullOrEmpty(value)) return 0;
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return int.Parse(value);
        }

        public static Dictionary<string, string> Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return PlayFabSimpleJson.DeserializeObject<Dictionary<string, string>>(data);
        }
        
        public static List<T> DeserializeToList<T>(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return PlayFabSimpleJson.DeserializeObject<List<T>>(data);
        }

        public static List<string> SplitString(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            
            var list = data.Split(';').ToList();
            var listCut = new List<string>();
            
            foreach (var value in list)
            {
                if(string.IsNullOrEmpty(value)) continue;
                if(string.IsNullOrWhiteSpace(value)) continue;
                listCut.Add(value);
            }

            return listCut;
        }

        private static string q = "\"";
        public static string DictionaryToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary
                .Select(kv 
                    => q + kv.Key + q + ":" + q + kv.Value + q)
                .ToArray()) + "}";
        }

        public static string RemoveTextChars(string text, string removingChars)
        {
            // Example: (Clone) as removingChars
            return text.Replace(removingChars, "");
        }

        public static string BoolToString(bool state)
        {
            if (state) return "true";
            else return "false";
        }

        public static bool StringToBool(string state)
        {
            if (state == "true") return true;
            if (state == "True") return true;
            if (state == "false") return false;
            if (state == "False") return false;
            Debug.LogError("Failed to convert string to bool | String: " + state);
            return false;
        }

        public static int GetInt(int? value)
        {
            if(value.HasValue) return value.Value;
            else return 0;
        }

        public static int GetIntFromString(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return int.Parse(value);
        }
        
        public static ItemInfo.Currency GetCurrencyType(string currencyName)
        {
            if (currencyName == ItemInfo.Currency.BC.ToString())
                return ItemInfo.Currency.BC;
            
            if (currencyName == ItemInfo.Currency.GL.ToString())
                return ItemInfo.Currency.GL;

            return ItemInfo.Currency.BC;
        }

        public static int GetItemsPrice(List<Item> requestItems, ItemInfo.Currency currencyType)
        {
            var totalPrice = 0;
            
            foreach (var item in requestItems)
            {
                totalPrice += item.info.GetItemPrice(currencyType);
            }

            return totalPrice;
        }

        public static List<string> GetItemNamesFromItemList(List<Item> itemList)
        {
            if (itemList == null || itemList.Count == 0) return null;

            var list = new List<string>();
            
            foreach (var item in itemList)
            {
                list.Add(item.info.itemName);
            }

            return list;
        }

        public static List<T> GetChildrens<T>(GameObject obj)
        {
            return ArrayToList(obj.GetComponentsInChildren<T>());
        }

        public static List<T> ArrayToList<T>(T[] array)
        {
            return array.OfType<T>().ToList();
        }

        public static int GetOppositeLayer(UnitFraction.Fraction fraction)
        {
            if (fraction == UnitFraction.Fraction.Red)
            {
                return LayerMask.NameToLayer(UnitFraction.Fraction.Blue.ToString());
            }
            
            if (fraction == UnitFraction.Fraction.Blue)
            {
                return LayerMask.NameToLayer(UnitFraction.Fraction.Red.ToString());
            }

            return LayerMask.NameToLayer("Player");
        }
    }
}

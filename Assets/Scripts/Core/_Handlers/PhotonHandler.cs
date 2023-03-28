using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Models;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Playstel
{
    public static class PhotonHandler 
    {
        public enum Hash
        {
            Health, MaxHealth, Lives, MaxLives, Frags, Deaths,
            Backpack, BagSlots, 
            Skin, Character, Weapons, Support,
            Tokens, LocalCurrency, MainCurrency, 
            Level, Experience, MaxExperience, 
            MaxEndurance, RunSpeed, SprintSpeed,
            Fraction, 
            Holders, Bullets, Turnover
        }

        public static void UpdateHashtable(Hash key, object value, Player player = null)
        {
            if (!PhotonNetwork.InRoom) return;

            if (player == null)
            {
                player = PhotonNetwork.LocalPlayer;
            }
            
            var hash = player.CustomProperties;
            
            if (hash == null)
            {
                Debug.Log("Hashtable is null");
                hash = new Hashtable();
            }

            var keyString = key.ToString();

            if (hash.ContainsKey(keyString)) hash.Remove(keyString); 
            
            hash.Add(keyString, value);
            
            player.SetCustomProperties(hash);
        }

        public static T GetPlayerHash<T>(Hash key, Player player = null)
        {
            if (!PhotonNetwork.InRoom) return default;

            if (player == null) player = PhotonNetwork.LocalPlayer;
            var value = player.CustomProperties[key.ToString()];

            if (value == null) { return default; }

            return (T)value;
        }
        
        public static void ClearNetworkCustomProperties(Player player = null)
        {
            if (player == null) player = PhotonNetwork.LocalPlayer;
            
            player.CustomProperties.Clear();
        }
    }
}
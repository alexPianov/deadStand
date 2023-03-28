using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiLeaderboardSlot : MonoBehaviour
    {
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI playerFrags;
        public TextMeshProUGUI playerPlace;
        public TextMeshProUGUI playerDeaths;
        public Image fractionMark;
        public GameObject iconBestPlayer;
        
        [Header("Colors")]
        public Color fractionRed;
        public Color fractionBlue;

        public int playerFragsNum;
        public int playerDeathsNum;

        public int GetAverageScore()
        {
            return (playerFragsNum * 2) - playerDeathsNum;
        }
        
        
        public void SetPlayerInfo(Player player)
        {
            Debug.Log("SetPlayerInfo: " + player.NickName);
            SetPlayerName(player);
            SetPlayerFrags(player);
            SetPlayerDeaths(player);
            SetPlayerFractionIcon(player);
        }

        public void SetPlayerInfo(Unit unit)
        {
            SetPlayerName(unit.unitName);
            
            if (unit.TryGetComponent(out UnitAiStatistics statistics))
            {
                SetPlayerFrags(statistics.GetFrags());
                SetPlayerDeaths(statistics.GetDeaths());
            }
            
            SetPlayerFractionIcon(unit.Fraction.currentFraction);
        }
        
        public void SetPlayerPlace(int place)
        {
            playerPlace.text = place.ToString();

            iconBestPlayer.SetActive(place == 1);
        }

        private void SetPlayerName(Player player)
        {
            playerName.text = player.NickName;
        }
        
        private void SetPlayerName(string player)
        {
            playerName.text = player;
        }

        private void SetPlayerFrags(Player player)
        {
            var frags = PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Frags, player);
            SetPlayerFrags(frags);
        }

        private void SetPlayerFrags(int frags)
        {
            playerFragsNum = frags;
            if(playerFrags) playerFrags.text = frags.ToString();
        }

        private void SetPlayerDeaths(Player player)
        {
            var deaths = PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Deaths, player);
            SetPlayerDeaths(deaths);
        }

        private void SetPlayerDeaths(int deaths)
        {
            playerDeathsNum = deaths;
            if(playerDeaths) playerDeaths.text = deaths.ToString();
        }
        
        private void SetPlayerFractionIcon(Player player)
        {
            var fraction = PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Fraction, player);
            
            SetPlayerFractionIcon((UnitFraction.Fraction) fraction);
        }

        private void SetPlayerFractionIcon(UnitFraction.Fraction fractionName)
        {
            if (fractionName == UnitFraction.Fraction.Blue)
            {
                fractionMark.color = fractionBlue;
            }
            
            if (fractionName == UnitFraction.Fraction.Red)
            {
                fractionMark.color = fractionRed;
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class FractionPick : MonoBehaviour
    {
        public TextMeshProUGUI playersCount;
        private Dictionary<UnitFraction.Fraction, int> fractionPlayersCount = new();

        [Header("Left Fraction")]
        public Button leftFractionButton;
        public UnitFraction.Fraction leftFraction;
        
        [Header("Right Fraction")]
        public Button rightFractionButton;
        public UnitFraction.Fraction rightFraction;
        
        [Inject] private PlayerBootRoom _playerBootRoom;
        [Inject] private PlayerBootRoomAi _playerBootRoomAi;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        { 
            SetButtons();
            RefreshFractionsSize();
        }

        private float timer;
        private const int updateTime = 2;
        public void Update()
        {
            timer += Time.deltaTime;

            if (timer > updateTime)
            {
                timer = 0;
                RefreshFractionsSize();
            }
        }

        private void SetButtons()
        {
            rightFractionButton.onClick.AddListener(PickRight);
            leftFractionButton.onClick.AddListener(PickLeft);
        }

        private async void PickRight()
        {
            _cacheAudio.PlayMusic(CacheAudio.Music.Null);
            _cacheAudio.Play(CacheAudio.MenuSound.OnCrush);
            await _playerBootRoom.CreatePlayer((int)rightFraction);

            Destroy(gameObject);
        }

        private async void PickLeft()
        {
            _cacheAudio.PlayMusic(CacheAudio.Music.Null);
            _cacheAudio.Play(CacheAudio.MenuSound.OnCrush);
            await _playerBootRoom.CreatePlayer((int)leftFraction);
            
            Destroy(gameObject);
        }

        private void RefreshFractionsSize()
        {
            var players = PhotonNetwork.PlayerList.ToList();
            
            if (players.Count == 0)
            {
                Debug.Log("Players count in room is null");
                return;
            }
            
            foreach(var player in players)
            {
                var fractionNumber = PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Fraction, player);
                var fractionName = (UnitFraction.Fraction) fractionNumber;
                if(fractionName == UnitFraction.Fraction.Null) continue;
                AddPlayerToFractionCount(fractionName);
            }

            UpdateFractionSize();
        }

        private void AddPlayerToFractionCount(UnitFraction.Fraction fraction)
        {
            if (fractionPlayersCount.ContainsKey(fraction))
            {
                fractionPlayersCount.TryGetValue(fraction, out int value);
                fractionPlayersCount.Remove(fraction);
                fractionPlayersCount.Add(fraction, value++);
            }
            else
            {
                fractionPlayersCount.Add(fraction, 1);
            }
        }

        private int recentLeft;
        private int recentRight;
        private void UpdateFractionSize()
        {
            var left = 0;
            var right = 0;
            
            if (fractionPlayersCount.ContainsKey(rightFraction))
            {
                fractionPlayersCount.TryGetValue(rightFraction, out var rightFractionPlayers);
                right = rightFractionPlayers;
            }

            if (fractionPlayersCount.ContainsKey(leftFraction))
            {
                fractionPlayersCount.TryGetValue(leftFraction, out var leftFractionPlayers);
                left = leftFractionPlayers;
            }
            
            if(left == recentLeft && right == recentRight) return;

            recentLeft = left;
            recentRight = right;
            
            playersCount.text = left + " : " + right;

            BlockFractionButtons(right, left);
        }
        
        private void BlockFractionButtons(int right, int left)
        {
            if (right > left) rightFractionButton.interactable = false;
            if (right < left) leftFractionButton.interactable = false;
        }
    }
}
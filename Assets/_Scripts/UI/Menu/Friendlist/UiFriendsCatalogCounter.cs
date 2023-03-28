using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiFriendsCatalogCounter : MonoBehaviour
    {
        public UiFriends.Status status;
        public TextMeshProUGUI catalogTitleText;
        
        [Inject] private CacheUserFriends _cacheUserFriends;

        private string startTitleText;

        private void Awake()
        {
            startTitleText = catalogTitleText.text;
        }

        public void RefreshCounter()
        {
            catalogTitleText.text = startTitleText;
            catalogTitleText.text += NewFriendsCount();
        }

        private string NewFriendsCount()
        {
            var users = _cacheUserFriends.GetPlayFabFriends(status);
            if (users == null || users.Count == 0) return "";
            return " (" + users.Count + ")";
        }
    }
}
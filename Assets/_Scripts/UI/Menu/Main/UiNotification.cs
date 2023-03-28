using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiNotification : MonoBehaviour
    {
        public Image notificationIcon;
        public Type currentType;

        [Inject] private CacheUserFriends _cacheUserFriends;
        [Inject] private CacheUserInfo _cacheUserInfo;
        
        public enum Type
        {
            NewItems, NewFriends
        }

        private void Awake()
        {
            notificationIcon.enabled = false;
        }

        private void Start()
        {
            if (currentType == Type.NewItems)
            {
                notificationIcon.enabled = CheckNewItems();
            }

            if (currentType == Type.NewFriends)
            {
                notificationIcon.enabled = CheckNewFriends();
            }
        }

        private bool CheckNewFriends()
        {
            var users = _cacheUserFriends.GetPlayFabFriends(UiFriends.Status.Pending);
            if (users == null || users.Count == 0) return false;
            return true;
        }

        private bool CheckNewItems()
        {
            var items = _cacheUserInfo.inventory.GetGrantedItems();
            if (items == null || items.Count == 0) return false;
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiCustomizeNotifications : MonoBehaviour
    {
        private List<UiCustomizeButton> customizedButtons = new();

        [Inject] private CacheUserInfo _cacheUserInfo;

        private void Awake()
        {
            customizedButtons = GetComponentsInChildren<UiCustomizeButton>().ToList();
        }

        private void Start()
        {
            UpdateNotifications();
        }

        public void UpdateNotifications()
        {
            CloseSubclassNotifications();

            var items = _cacheUserInfo.inventory.GetGrantedItems();

            foreach (var item in items)
            {
                if(item) ActiveNotificationIcon(item.info.ItemSubclass);
            }
        }

        private void ActiveNotificationIcon(ItemInfo.Subclass itemSubclass)
        {
            foreach (var customizedButton in customizedButtons)
            {
                if (customizedButton.currentSubclass == itemSubclass)
                {
                    customizedButton.NewItemInCatalog(true);
                }
            }
        }

        private void CloseSubclassNotifications()
        {
            foreach (var customizedButton in customizedButtons)
            {
                customizedButton.NewItemInCatalog(false);
            }
        }
    }
}
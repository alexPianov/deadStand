using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiKillerBoot : UIElementLoadByType
    {
        public GameObject killerPrefab;
        public TextMeshProUGUI killerName;
        
        [Inject]
        private Unit _unit;

        [Inject]
        private CacheAudio _cacheAudio;

        private void Start()
        {
            var killerUnit = _unit.DeathScreen.GetLastKillerUnit();

            killerName.text = "Killed by " + killerUnit.photonView.Owner.NickName;

            var killerItems = new List<Item>();
            
            killerItems.AddRange(killerUnit.Items.GetItemList(ItemInfo.Catalog.Character));
            killerItems.AddRange(killerUnit.Items.GetItemList(ItemInfo.Catalog.Weapons));

            var killerItemNames = new List<string>();
            
            foreach (var item in killerItems)
            {
                killerItemNames.Add(item.info.itemName);
            }
            
            var killerItemsNew = _unit.CacheItemInfo
                .CreateItemList(killerItemNames.ToArray());
            
            var killerAvatarInstance = CreateKillerInstance();
            
            if (killerAvatarInstance.TryGetComponent(out UnitBuilder builder))
            {
                builder.CreateItemsData(false);
                builder.AddItems(killerItemsNew);
            }
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnGetItem);
            
            Invoke(nameof(Close), 2f);
        }
        
        private GameObject CreateKillerInstance()
        {
            return Instantiate
                (killerPrefab, transform.position, transform.rotation, transform);
        }

        private void Close()
        {
            Load(Elements.GUI, UIElementContainer.Type.Screen);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using Playstel.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class UiCharacterSlot : MonoBehaviour
    {
        public Image characterImage;
        public TextMeshProUGUI itemName;
        private CatalogItem _catalogItem;
        private Transform _focusTransform;

        [Inject] private CacheSprites _cacheSprites;
        private UiCharacterPick _pick;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Active);
        }
        
        public void SetCharacterInfo(ItemInfo info)
        { 
            _catalogItem = info.GetCatalogItem();
            
            itemName.text = info.itemName;
            characterImage.sprite = GetSchemeSprite(info);
        }

        private Sprite GetSchemeSprite(ItemInfo info)
        {
            var spriteName = info.GetUnsafeValue("Sprite");
            
            return _cacheSprites
                .GetSpriteFromAtlas(spriteName, ItemInfo.Catalog.Character);
        }

        public void SetFocusTransform(Transform focusTransform)
        {
            _focusTransform = focusTransform;
        }

        public void SetCharacterActivator(UiCharacterPick characterPick)
        {
            _pick = characterPick;
        }

        public void SetSlotAsPickedAtStart()
        {
            SetFocus();
            
            _pick.SetPickedCharacter(_catalogItem);
            _pick.UpdateCustomizeButtons();
            
            transform.SetSiblingIndex(0);
        }

        public void Active()
        {
            _pick.Pick(_catalogItem);
            SetFocus();
        }

        private void SetFocus()
        {
            _focusTransform.SetParent(transform, false);
            _focusTransform.localPosition = new Vector3(0, 0, 0);
            _focusTransform.SetSiblingIndex(0);
        }

        public CatalogItem GetCatalogItem()
        {
            return _catalogItem;
        }
    }   
}

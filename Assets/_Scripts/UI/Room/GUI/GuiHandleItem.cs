using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(Button))]
    public class GuiHandleItem : UiTransparency
    {
        public TextMeshProUGUI itemName;
        public Image itemImage;

        private Item _currentItem;
        [Inject] private Unit _unit;
        [Inject] private CacheAudio _cacheAudio;
        
        public async void SetItemInfo(Item item)
        {
            _currentItem = item;
            
            if (item.info.itemCatalog == ItemInfo.Catalog.Weapons)
            {
                await WeaponSetup(item);
            }
            
            if (item.info.itemCatalog == ItemInfo.Catalog.Support)
            {
                GetComponent<Button>().onClick.AddListener(UseSupportItem);
            }

            itemImage.sprite = item.info.itemSprite;
            
            Transparency(false);

            if (_unit.HandleItems.currentItem == item)
            {
                SetFocus();
            }
        }

        private async UniTask WeaponSetup(Item item)
        {
            GetComponent<Button>().onClick.AddListener(Pick);

            itemName.text = item.info.itemName;

            if (item.info.itemClass == ItemInfo.Class.Firearm)
            {
                var firearm = _unit.HandleItems.GetFirearm();

                await GetComponentInChildren<GuiHandleItemBullets>().SetFirearm(firearm);
            }
            else
            {
                Destroy(GetComponentInChildren<GuiHandleItemBullets>().gameObject);
            }
        }

        private float destroyTime = 0.2f;
        public void UseSupportItem()
        {
            if(!_unit.photonView.IsMine) return;

            if (_currentItem.info == null)
            {
                Debug.Log("Item info is null");
                return;
            }
            
            if (_currentItem.info.itemClass == ItemInfo.Class.Drug)
            {
                if (_unit.Drugs.OnDragEffect())
                {
                    _cacheAudio.Play(CacheAudio.MenuSound.OnBack);
                    return;
                }
                
                if (_currentItem.info.ItemSubclass == ItemInfo.Subclass.Health)
                {
                    if (_unit.Health.GetUnitHealth() == _unit.Health.GetMaxHealth())
                    {
                        _cacheAudio.Play(CacheAudio.MenuSound.OnBack);
                        return;
                    } 
                }
            }

            GetComponent<Button>().interactable = false;
            Transparency(true);
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnUse);
            
            var request = HandlerHostRequest.GetUseRequest(_currentItem);
            _unit.HostOperator.Run(UnitHostOperator.Operation.Use, request);

            Destroy(gameObject, destroyTime);
        }

        public void Pick()
        {
            if (_unit.Await.IsAwaiting()) return;

            if (_currentItem == _unit.HandleItems.currentItem)
            {
                ReloadCurrentWeapon(); return;
            }
            
            _unit.HandleItems.PickItem(_currentItem);
            
            SetFocus();
        }

        private void ReloadCurrentWeapon()
        {
            var firearm = _unit.HandleItems.GetFirearm();
            
            if (firearm == null) return;

            if (firearm.Stat.eachShotReload) return;

            if (firearm.Ammo.GetAmmoInfo().itemName == "Fuel") return;

            _unit.Callback.FirearmReload();
        }

        private void SetFocus()
        {
            transform.parent.GetComponent<GuiButtonGroup>().MoveFocus(transform);
        }
    }
}

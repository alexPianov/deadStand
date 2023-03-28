using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitBuffer : MonoBehaviourPun
    {
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private CacheAudio _cacheAudio;
        [Inject] private HandlerPulse _handlerPulse;

        private Unit _unit;
        public void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        private int GetNewValue(int income, int recentValue, bool increment)
        {
            if (increment) recentValue += income;
            else recentValue -= income;
            
            return recentValue;
        }
        
        // HEALTH:
        
        #region Change Health

        public void ChangeHealth(int income, bool increment = false, bool updateHashtable = true, bool updateUi= true)
        {
            var recentValue = _unit.Health.GetUnitHealth();
            var newValue = GetNewValue(income, recentValue, increment);
            
            photonView.RPC(nameof(RPC_ChangeHealth), RpcTarget.All, newValue, updateHashtable, updateUi);
        }

        [PunRPC]
        private void RPC_ChangeHealth(int value, bool updateHashtable = true, bool updateUi = true)
        {
            _unit.Health.UpdateHealth(value, updateUi);

            if (updateHashtable && _unit.photonView.IsMine && !_unit.IsNPC)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Health, 
                    value, _unit.photonView.Owner);
            }
        }

        #endregion

        // RESOURCES
        
        #region Change Lives
        
        public void ChangeLives(int income = 1, bool increment = false)
        {
            if(_unit.IsNPC) return;
            
            var recentValue = _unit.Lives.GetLives();
            var newValue = GetNewValue(income, recentValue, increment);
            
            photonView.RPC(nameof(RPC_ChangeLives), RpcTarget.All, newValue);
        }

        [PunRPC]
        private void RPC_ChangeLives(int value)
        {
            _unit.Lives.UpdateLives(value);
            
            if (_unit.photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Lives, 
                    value, _unit.photonView.Owner);
            }
        }

        #endregion
        
        #region Change Currency

        public void ChangeCurrency(int income, bool increment = false, ItemInfo.Currency currency = ItemInfo.Currency.BC)
        {
            var recentValue = _unit.Currency.GetUnitCurrency(currency);
            var newValue = GetNewValue(income, recentValue, increment);

            photonView.RPC(nameof(RPC_ChangeCurrency), RpcTarget.All, newValue, currency.ToString());
        }
        
        [PunRPC]
        private async void RPC_ChangeCurrency(int value, string currencyName)
        {
            _unit.Currency.UpdateCurrency(value, currencyName);
            
            if (_unit.photonView.IsMine)
            {
                if (currencyName == ItemInfo.Currency.BC.ToString())
                {
                    PhotonHandler.UpdateHashtable(PhotonHandler.Hash.LocalCurrency,
                        value, _unit.photonView.Owner);
                }
                
                if (currencyName == ItemInfo.Currency.GL.ToString())
                {
                    PhotonHandler.UpdateHashtable(PhotonHandler.Hash.MainCurrency,
                        value, _unit.photonView.Owner);
                    
                    var args = new
                    {
                        NewCurrencyValue = value
                    };

                    var result = await PlayFabHandler.ExecuteCloudScript
                        (PlayFabHandler.Function.UpdateMainCurrency, args);

                    if (result == null)
                    {
                        _handlerPulse.OpenTextNote("Unauthorized amount of currency");
                    }
                }
            }
        }
        
        #endregion
        
        #region Change Tokens (From Master)

        public void ChangeTokens(int income = 1, bool increment = true)
        {
            var recentValue = _unit.Tokens.GetTokensCount();
            var newValue = GetNewValue(income, recentValue, increment);
            
            photonView.RPC(nameof(RPC_ChangeTokens), RpcTarget.All, newValue);
        }

        [PunRPC]
        private void RPC_ChangeTokens(int value)
        {
            Debug.Log("RPC_ChangeTokens: " + value + " for " + photonView.Owner.NickName);
            _unit.Tokens.UpdateTokens(value);
            
            if (_unit.photonView.IsMine && !_unit.IsNPC)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Tokens, 
                    value, _unit.photonView.Owner);
            }
        }

        #endregion

        // STATISTICS
        
        #region Change Frags

        public void ChangeFrags(int income = 1)
        {
            var recentValue = StatHandler.GetFrags(_unit.photonView.Owner);
            
            var newValue = GetNewValue(income, recentValue, true);
            
            Debug.Log("New Frag Income: " + income + " | Change to: " + newValue);
            
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Frags, 
                newValue, _unit.photonView.Owner);
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnFrag);
        }

        #endregion
        
        #region Change Experience

        public void ChangeExperience(int income = 0)
        {
            var recentValue = StatHandler.GetExperience(_unit.photonView.Owner);
            var newValue = GetNewValue(income, recentValue, true);
            
            photonView.RPC(nameof(RPC_ChangeExperience), RpcTarget.All, newValue);
        }

        [PunRPC]
        private void RPC_ChangeExperience(int value)
        {
            _unit.Experience.UpdateExperience(value);
        }

        #endregion
        
        #region Change Level

        [ContextMenu("Add Level")]
        public void AddLevel()
        {
            ChangeLevel(1);
        }
        
        public void ChangeLevel(int income = 0)
        {
            if(!photonView.IsMine) return;

            var recentValue = StatHandler.GetLevel(_unit.photonView.Owner);
            var newValue = GetNewValue(income, recentValue, true);
            
            photonView.RPC(nameof(RPC_ChangeLevel), RpcTarget.All, newValue);
        }

        [PunRPC]
        private void RPC_ChangeLevel(int value)
        {
            _unit.Experience.UpdateLevel(value);
            
            if (_unit.photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Level, 
                    value, _unit.photonView.Owner);
            }
        }

        #endregion

        // INVENTORY
        
        #region Change Items (From Master)

        public void ChangeItems(string[] incomeItems, bool increment, ItemInfo.Catalog catalog)
        {
            if (incomeItems == null) return;

            if (catalog == ItemInfo.Catalog.Weapons)
            {
                photonView.RPC(nameof(RPC_ChangeWeaponItems), RpcTarget.All, 
                    incomeItems, increment);
                
                return;
            }

            if (catalog == ItemInfo.Catalog.Backpack)
            {
                photonView.RPC(nameof(RPC_ChangeBackpackItems), RpcTarget.All, 
                    incomeItems, increment);
                
                return;
            }

            var inventoryItems = _unit.Items.GetItemList(catalog);
            var inventoryItemNames = DataHandler.GetItemNamesFromItemList(inventoryItems);
            var newItemNamesArray = EditItemNames(incomeItems, increment, inventoryItemNames);
            
            if (catalog == ItemInfo.Catalog.Support)
            {
                photonView.RPC(nameof(RPC_ChangeSupportItems), RpcTarget.All, 
                    newItemNamesArray as object);
            }

            if (catalog == ItemInfo.Catalog.Character)
            {
                photonView.RPC(nameof(RPC_ChangeCharacterItems), RpcTarget.All, 
                    newItemNamesArray as object);
            }
        }

        [PunRPC]
        private void RPC_ChangeCharacterItems(object itemsNames)
        {
            var itemsArray = (string[]) itemsNames;
            
            var newItems = _cacheItemInfo
                .CreateItemList(itemsArray, ItemInfo.Catalog.Character);
            
            _unit.Items.RemoveItems(ItemInfo.Catalog.Character);
            _unit.Builder.AddItems(newItems);
            
            if (_unit.photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Character, 
                    itemsArray, _unit.photonView.Owner);
            }
        }
        
        [PunRPC]
        private void RPC_ChangeWeaponItems(object itemsNames, bool increment)
        {
            var itemsArray = (string[]) itemsNames;
            
            if (!increment)
            {
                var items = _unit.Items.GetItems(itemsArray.ToList());
                _unit.Items.RemoveItems(items);
                return;
            }

            var newItems = _cacheItemInfo
                .CreateItemList(itemsArray, ItemInfo.Catalog.Weapons);

            _unit.Builder.AddItems(newItems);
            
            PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Weapons, 
                itemsArray, _unit.photonView.Owner);
        }
        
        [PunRPC]
        private void RPC_ChangeSupportItems(object itemsNames)
        {
            var itemsArray = (string[]) itemsNames;
            
            _unit.Items.RemoveItems(ItemInfo.Catalog.Support);
            
            var newItems = _cacheItemInfo
                .CreateItemList(itemsArray, ItemInfo.Catalog.Support);
            
            _unit.Builder.AddItems(newItems);
            
            if (photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Support, 
                    itemsArray, photonView.Owner);
            }
        }
        
        [PunRPC]
        private void RPC_ChangeBackpackItems(object itemsNames, bool increment)
        {
            var itemsArray = (string[]) itemsNames;

            Debug.Log("RPC_ChangeBackpackItems: " + itemsArray.Length + " for " + photonView.Owner.NickName);
            
            if (!increment)
            {
                var items = _unit.Items.GetItems(itemsArray.ToList());
                _unit.Items.RemoveItems(items);
                return;
            }
            
            var newItems = _cacheItemInfo
                .CreateItemList(itemsArray, ItemInfo.Catalog.Backpack);

            _unit.Items.RemoveItems(ItemInfo.Catalog.Backpack);
            _unit.Builder.AddItems(newItems);
            
            if (_unit.photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Backpack, 
                    itemsArray, _unit.photonView.Owner);
            }
        }

        private string[] EditItemNames(string[] incomeItems, bool increment, List<string> inventoryItems)
        {
            if (inventoryItems == null) inventoryItems = new List<string>(); 

            foreach (var incomeItem in incomeItems)
            {
                if (increment)
                {
                    inventoryItems.Add(incomeItem);
                }
                else
                {
                    if (inventoryItems.Contains(incomeItem))
                    {
                        inventoryItems.Remove(incomeItem);
                    }
                }
            }

            return inventoryItems.ToArray();
        }

        #endregion

        #region Remove Current Weapon

        

        #endregion

        #region Change Turnover

        public void ChangeTurnover(int income = 1)
        {
            if(!photonView.IsMine) return;

            var recentValue = StatHandler.GetTurnover(_unit.photonView.Owner);
            
            var newValue = GetNewValue(income, recentValue, true);
            
            photonView.RPC(nameof(RPC_ChangeTurnover), RpcTarget.All, newValue);
        }

        [PunRPC]
        private void RPC_ChangeTurnover(int value)
        {
            if (_unit.photonView.IsMine)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Turnover, 
                    value, _unit.photonView.Owner);
            }
        }

        #endregion
        
        #region Change Holders

        public void ChangeHolders(int income = 1, bool increment = false)
        {
            if(!photonView.IsMine) return;

            var recentValue = _unit.Ammo.GetCurrentAmmo().Payload.GetHolders();
            var newValue = GetNewValue(income, recentValue, increment);
            
            photonView.RPC(nameof(RPC_ChangeHolders), RpcTarget.All, newValue);
        }

        [PunRPC]
        private async void RPC_ChangeHolders(int value)
        {
            if (_unit.photonView.IsMine && !_unit.IsNPC)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Holders, 
                    value, _unit.photonView.Owner);
            }
            
            await WaitForAmmoPayload();

            _unit.Ammo.GetCurrentAmmo().Payload.UpdateHolders(value);
        }
        
        #endregion
        
        #region Change Bullets

        public void ChangeBullets(int income = 1)
        {
            if(!photonView.IsMine) return;
            
            photonView.RPC(nameof(RPC_ChangeBullets), RpcTarget.All, income);
        }

        [PunRPC]
        private async void RPC_ChangeBullets(int value)
        {
            if (_unit.photonView.IsMine && !_unit.IsNPC)
            {
                PhotonHandler.UpdateHashtable(PhotonHandler.Hash.Bullets, 
                    value, _unit.photonView.Owner);
            }
            
            await WaitForAmmoPayload();
            
            _unit.Ammo.GetCurrentAmmo().Payload.UpdateBullets(value);
        }

        private async UniTask WaitForAmmoPayload()
        {
            await UniTask.WaitUntil(() => _unit);
            await UniTask.WaitUntil(() => _unit.Ammo);
            await UniTask.WaitUntil(() => _unit.Ammo.GetCurrentAmmo());
            await UniTask.WaitUntil(() => _unit.Ammo.GetCurrentAmmo().Payload);
        }

        #endregion
        
    }
}
using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitHostOperator : MonoBehaviourPun
    {
        [Inject] private CacheTitleData _cacheTitleData;
        [Inject] private CacheItemInfo _cacheItemInfo;
        [Inject] private HandlerPulse _handlerPulse;

        private Unit _unit;
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        #region Operation

        public Status _status;
        public Operation _currentOperation;

        public enum Status
        {
            Success, Reject, Request, None
        }
        
        public enum Operation
        {
            Sell, Loot, Remove, Buy, 
            Respawn, Token, Spin, Use
        }

        public async UniTask<Status> Run(Operation operation, Dictionary<string, string> request)
        {
            Debug.Log("Run operation: " + operation);

            if (!photonView.IsMine) return Status.None;
            
            if (_status == Status.Request)
            {
                Debug.Log("Run " + operation  + " | Warning! " +
                          "Privious operation is still running: " + _currentOperation);
            }

            _currentOperation = operation;

            _status = Status.Request;

            photonView.RPC(operation.ToString(), photonView.Owner, request);
            
            // photonView.RPC(operation.ToString(), RpcTarget.All, 
            //     request, photonView.ViewID);

            await UniTask.WaitUntil(() => _status != Status.Request);
            return _status;
        }

        private bool SendResult(Unit otherClient, bool result, string reason = null)
        {
            photonView.RPC(nameof(RPC_GetResult), otherClient.photonView.Owner, result, reason);
            return result;
        }

        [PunRPC]
        private void RPC_GetResult(bool state, string reason)
        {
            ResultStatus(state, reason);
        }
        
        private void ResultStatus(bool state, string reason = null)
        {
            _status = state ? Status.Success : Status.Reject;
            
            Debug.Log("Host operation success");

            if (_status == Status.Reject)
            {
                Debug.Log("Host operation reject: " + reason);
                
                if(!string.IsNullOrEmpty(reason)) _handlerPulse.OpenTextNote(reason);
            }
        }

        #endregion

        #region Handler

        private static string GetRequestValue(Dictionary<string, string> request, HandlerHostRequest.ItemData itemData)
        {
            request.TryGetValue(itemData.ToString(), out string itemName);
            return itemName;
        }
        
        private Unit GetUnit(int clientId)
        {
            return _unit;
            var unit = PhotonView.Find(clientId).GetComponent<Unit>();
            if(unit == null) { Debug.Log("The client does not exist anymore: " + clientId);}
            return unit;
        }

        private bool CheckItemsAvailable(List<string> requestItemNames, Unit clientUnit)
        {
            if (requestItemNames == null || requestItemNames.Count == 0)
            {
                Debug.Log("Request items is null: " + clientUnit.photonView.Owner.NickName);
                return SendResult(clientUnit, false);
            }

            var unitRequestItems = clientUnit.Items.GetItems(requestItemNames);
            
            if (unitRequestItems == null || unitRequestItems.Count == 0)
            {
                return SendResult(clientUnit, false, "Request items is null: " + clientUnit.photonView.Owner.NickName);
            }
            
            if (requestItemNames.Count != unitRequestItems.Count)
            {
                return SendResult(clientUnit, false, "Requested items count is doesn't match with host: " + clientUnit.photonView.Owner.NickName);
            }

            return true;
        }
        
        private bool FundsEnougt(int totalPrice, ItemInfo.Currency currencyType, Unit clientUnit)
        {
            var playerCurrency = clientUnit.Currency.GetUnitCurrency(currencyType);

            Debug.Log("Total Price: " + totalPrice + " | Player Currency: " + playerCurrency);
            
            if (totalPrice > playerCurrency)
            {
                return SendResult(clientUnit, false, "Insufficient funds: " + clientUnit.photonView.Owner.NickName);
            }

            return true;
        }
        
        private List<string> GetRequestItemNames(Dictionary<string, string> request)
        {
            var itemNameListString = GetRequestValue(request, HandlerHostRequest.ItemData.Items);
            if (string.IsNullOrEmpty(itemNameListString)) return null;
            if (string.IsNullOrWhiteSpace(itemNameListString)) return null;
            var splitedList = DataHandler.SplitString(itemNameListString);
            if (splitedList == null || splitedList.Count == 0) return null;
            return splitedList;
        }
        
        private ItemInfo.Catalog GetRequestedCatalog(Dictionary<string, string> request)
        {
            var catalog = GetRequestValue(request, HandlerHostRequest.ItemData.Catalog);
            
            if (string.IsNullOrEmpty(catalog))
            {
                Debug.Log("Attention! Requested catalog is null");
                return ItemInfo.Catalog.Null;
            }
            
            return KeyStore.GetCatalog(catalog);
        }

        private ItemInfo.Currency GetRequestedCurrency(Dictionary<string, string> request)
        {
            var currency = GetRequestValue(request, HandlerHostRequest.ItemData.Currency);
            if (string.IsNullOrEmpty(currency)) return ItemInfo.Currency.BC;
            return DataHandler.GetCurrencyType(currency);
        }

        private int GetRequestedItemsPrice(List<string> items, ItemInfo.Catalog catalog, ItemInfo.Currency currency)
        {
            var itemList = _cacheItemInfo.CreateItemList(items.ToArray(), catalog);
            if (itemList == null || itemList.Count == 0) return 0;
            return DataHandler.GetItemsPrice(itemList, currency);
        }

        private int GetRequestedTokensCount(Dictionary<string, string> request)
        {
            var tokensString = GetRequestValue(request, HandlerHostRequest.ItemData.Tokens);
            if (string.IsNullOrEmpty(tokensString)) return 0;
            return int.Parse(tokensString);
        }

        #endregion
        
        // OPERATIONS

        #region Sell

        private ObscuredInt collectibleRate = 3;
        [PunRPC]
        private void Sell(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            
            if (!clientUnit) return; 
            
            var requestItemNames = GetRequestItemNames(request);

            if (!CheckItemsAvailable(requestItemNames, clientUnit)) return;

            var catalog = GetRequestedCatalog(request);
            var currency = GetRequestedCurrency(request);

            clientUnit.Buffer.ChangeItems(requestItemNames.ToArray(), false, catalog);

            var totalPrice = GetRequestedItemsPrice(requestItemNames, catalog, currency);

            Debug.Log("totalPrice 1: " + totalPrice);
            
            if (IsCollectibleItems(request))
            { 
                Debug.Log("Is Collectible");
                totalPrice *= collectibleRate;
            } 
            
            Debug.Log("totalPrice 2: " + totalPrice);
            
            clientUnit.Buffer.ChangeCurrency(totalPrice, true);
            clientUnit.Buffer.ChangeTurnover(totalPrice);

            SendResult(clientUnit, true);
        }

        private static bool IsCollectibleItems(Dictionary<string, string> request)
        {
            var result = GetRequestValue(request, HandlerHostRequest.ItemData.Collectible);
            Debug.Log("IsCollectibleItems: " + result);
            if (string.IsNullOrEmpty(result)) return false;
            return bool.Parse(result);
        }

        #endregion
        
        #region Buy

        [PunRPC]
        private void Buy(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            
            if (clientUnit == null) return;
            
            var requestItemNames = GetRequestItemNames(request);
            
            if (requestItemNames == null || requestItemNames.Count == 0)
            {
                Debug.Log("Request items is null: " + clientUnit.photonView.Owner.NickName);
                SendResult(clientUnit, false); return;
            }

            var catalog = GetRequestedCatalog(request);
            var currencyName = GetRequestedCurrency(request);
            var totalPrice = GetRequestedItemsPrice(requestItemNames, catalog, currencyName);

            if (!FundsEnougt(totalPrice, currencyName, clientUnit)) return;

            foreach (var requestedItem in requestItemNames)
            {
                Debug.Log("Purchasing Item Name: " + requestedItem);
            }

            Debug.Log("Buy: " + requestItemNames[0] + " for " + totalPrice + " from catalog " + catalog + " | User: " + clientUnit.photonView.Owner.NickName);
            
            clientUnit.Buffer.ChangeItems(requestItemNames.ToArray(), true, catalog);

            clientUnit.Buffer.ChangeCurrency(totalPrice, false, currencyName); 
            
            SendResult(clientUnit, true);
        }

        #endregion
        
        #region Loot

        [PunRPC]
        private void Loot(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            
            if (clientUnit == null) return;
            
            var requestItemNames = GetRequestItemNames(request);

            if (!BagSlotsEnought(requestItemNames, clientUnit)) return;

            string[] requestItemsArray = {};
            
            if (requestItemNames != null)
            {
                requestItemsArray = requestItemNames.ToArray();
            }
            
            var catalog = GetRequestedCatalog(request);
            clientUnit.Buffer.ChangeItems(requestItemsArray, true, catalog);

            SendResult(clientUnit, true);
        }

        private bool BagSlotsEnought(List<string> requestItemNames, Unit clientUnit)
        {
            if (requestItemNames != null && requestItemNames.Count > clientUnit.Items.GetMaxBagSlots())
            {
                return SendResult(clientUnit, false, 
                    "Bag slot count (" + clientUnit.Items.GetMaxBagSlots() + ") is smaller than requested items count (" 
                    + requestItemNames.Count + ")");
            }

            return true;
        }

        #endregion

        #region Remove

        [PunRPC]
        private void Remove(Dictionary<string, string> request)
        {
            var clientUnit = _unit;

            if (!clientUnit) return;
            
            var requestItemNames = GetRequestItemNames(request);
            
            if (!CheckItemsAvailable(requestItemNames, clientUnit)) return; 
            
            var catalog = GetRequestedCatalog(request);

            clientUnit.Buffer.ChangeItems(requestItemNames.ToArray(), false, catalog);
            
            SendResult(clientUnit, true);
        }

        #endregion

        #region Token

        [PunRPC]
        private void Token(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            if (clientUnit == null) return;
            
            if (!CheckTokensCount(clientUnit, request)) return;
            
            var sellTokens = SellTokens(request);

            if (sellTokens)
            {
                var price = clientUnit.Tokens.GetTokensPrice();
                clientUnit.Buffer.ChangeCurrency(price, true);
                var recentValue = clientUnit.Tokens.GetTokensCount();
                clientUnit.Buffer.ChangeTokens(recentValue, false);
                clientUnit.Buffer.ChangeTurnover(price);
            }
            else
            {
                clientUnit.Buffer.ChangeTokens();
            }
            
            SendResult(clientUnit, true);
        }

        private static bool SellTokens(Dictionary<string, string> request)
        {
            var state = GetRequestValue(request, HandlerHostRequest.ItemData.SellTokens);
            if (string.IsNullOrEmpty(state)) return false;
            return bool.Parse(state);
        }

        private bool CheckTokensCount(Unit clientUnit, Dictionary<string, string> request)
        {
            var requestTokensCount = GetRequestedTokensCount(request);
            var clientTokensCount = clientUnit.Tokens.GetTokensCount();

            if (requestTokensCount != clientTokensCount)
            {
                return SendResult(clientUnit, false, "Client token count is doesn't match with host: " + clientUnit.photonView.Owner.NickName);
            }

            return true;
        }

        #endregion
        
        #region Spin

        [PunRPC]
        private void Spin(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            if (clientUnit == null) return;
            
            var spinPrice = _cacheTitleData.GetTitleDataInt
                (CacheTitleData.TitleDataKey.SpinPrice);
            
            var currencyName = GetRequestedCurrency(request);
            
            if(!FundsEnougt(spinPrice, currencyName, clientUnit)) return;

            var catalog = GetRequestedCatalog(request);
            var itemName = GetRequestValue(request, HandlerHostRequest.ItemData.Item);
            
            if (!string.IsNullOrEmpty(itemName))
            {
                string[] itemArray = { itemName };
                clientUnit.Buffer.ChangeItems(itemArray, true, catalog);
            }
            
            clientUnit.Buffer.ChangeCurrency(spinPrice, false, currencyName);
            
            SendResult(clientUnit, true);
        }
        
        #endregion

        #region Respawn

        [PunRPC]
        private void Respawn(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            
            if (!clientUnit) return;

            clientUnit.Callback.Respawn(KillerId(request));
                
            var startHealth = GetStartHealth(clientUnit);
            clientUnit.Buffer.ChangeHealth(startHealth, true);
                
            GetItems(clientUnit, ItemInfo.Catalog.Weapons);
            
            SendResult(clientUnit, true);
        }

        public static string[] GetItems(Unit clientUnit, ItemInfo.Catalog catalog)
        {
            var items = clientUnit.Items.GetItemList(catalog);
            if (items == null || items.Count == 0) return new string[] {};
            var itemNames = DataHandler.GetItemNamesFromItemList(items);
            if(itemNames == null || itemNames.Count == 0) return new string[] {};
            return itemNames.ToArray();
        }

        private static int GetStartHealth(Unit clientUnit)
        {
            return clientUnit.Health.GetMaxHealth();
        }

        private int KillerId(Dictionary<string, string> request)
        {
            var killerId = GetRequestValue(request, HandlerHostRequest.ItemData.Killer);
            if (string.IsNullOrEmpty(killerId)) return 0;
            return int.Parse(killerId);
        }

        #endregion
        
        #region Use 
        
        [PunRPC]
        private void Use(Dictionary<string, string> request)
        {
            var clientUnit = _unit;
            if (!clientUnit) return;
            
            var requestItemNames = GetRequestItemNames(request);
            
            Debug.Log("Use: " + requestItemNames[0]);
            
            if (!CheckItemsAvailable(requestItemNames, clientUnit)) return;
            
            //clientUnit.Callback.UseItems(requestItemNames.ToArray());
            
            SendResult(clientUnit, true);
        }

        #endregion
    }
}

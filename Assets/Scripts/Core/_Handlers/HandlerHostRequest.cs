using System.Collections.Generic;

namespace Playstel
{
    public static class HandlerHostRequest 
    {
        public enum ItemData
        {
            Item, Items, Collectible, ExternalId, Catalog, Class, Subclass, Currency, 
            Tokens, SellTokens, Killer
        }
        
        public static Dictionary<string,string> GetSellRequest(
            List<Item> itemList, 
            ItemInfo.Catalog itemCatalog,
            bool collectible = false, 
            ItemInfo.Currency itemCurrency = ItemInfo.Currency.BC)
        {
            string names = null;

            foreach (var item in itemList)
            {
                names += item.info.itemName + ";";
            }
            
            return new Dictionary<string, string>()
            {
                { ItemData.Items.ToString(), names },
                { ItemData.Catalog.ToString(), itemCatalog.ToString() },
                { ItemData.Collectible.ToString(), collectible.ToString() },
                { ItemData.Currency.ToString(), itemCurrency.ToString() }
            };
        }

        public static Dictionary<string,string> GetLootRequest(List<Item> selectedItems,
            ItemInfo.Catalog catalog)
        {
            string names = "";

            foreach (var item in selectedItems)
            {
                names += item.info.itemName + ";";
            }
            
            return new Dictionary<string, string>()
            {
                { ItemData.Items.ToString(), names },
                { ItemData.Catalog.ToString(), catalog.ToString() }
            };
        }
        
        public static Dictionary<string,string> GetRemoveRequest(
            List<Item> selectedItems, ItemInfo.Catalog catalog)
        {
            string names = "";

            foreach (var item in selectedItems)
            {
                names += item.info.itemName + ";";
            }
            
            return new Dictionary<string, string>()
            {
                { ItemData.Items.ToString(), names },
                { ItemData.Catalog.ToString(), catalog.ToString() }
            };
        }

        public static Dictionary<string, string> GetBuyRequest(
            ItemInfo item, ItemInfo.Currency currency, ItemInfo.Catalog catalog)
        {
            var items = new List<ItemInfo>();
            items.Add(item);
            return GetBuyRequest(items, currency, catalog);
        }
        
        public static Dictionary<string,string> GetBuyRequest(
            List<ItemInfo> selectedItems, ItemInfo.Currency currency, ItemInfo.Catalog catalog)
        {
            string names = "";

            foreach (var item in selectedItems)
            {
                names += item.itemName + ";";
            }
            
            return new Dictionary<string, string>()
            {
                { ItemData.Items.ToString(), names },
                { ItemData.Currency.ToString(), currency.ToString() },
                { ItemData.Catalog.ToString(), catalog.ToString() }
            };
        }

        public static Dictionary<string, string> GetUseRequest(Item item)
        {
            var items = new List<Item>();
            items.Add(item);
            return GetUseRequest(items);
        }
        
        public static Dictionary<string, string> GetUseRequest(List<Item> selectedItems)
        {
            string names = "";

            foreach (var item in selectedItems)
            {
                names += item.info.itemName + ";";
            }

            return new Dictionary<string, string>()
            {
                { ItemData.Items.ToString(), names }
            };
        }

        public static Dictionary<string, string> GetCasinoSpinRequest(string itemName, 
            ItemInfo.Catalog itemCatalog,
            ItemInfo.Currency currency)
        {
            return new Dictionary<string, string>()
            {
                { ItemData.Item.ToString(), itemName },
                { ItemData.Catalog.ToString(), itemCatalog.ToString() },
                { ItemData.Currency.ToString(), currency.ToString() }
            };
        }
        
        public static Dictionary<string,string> GetTokenRequest(
            int tokenCount, bool sellToken = false)
        {
            return new Dictionary<string, string>()
            {
                { ItemData.Tokens.ToString(), tokenCount.ToString() },
                { ItemData.SellTokens.ToString(), sellToken.ToString() }
            };
        }

        public static Dictionary<string,string> GetRespawnRequest(int killerId)
        {
            return new Dictionary<string, string>()
            {
                { ItemData.Killer.ToString(), killerId.ToString() }
            };
        }
    }
}

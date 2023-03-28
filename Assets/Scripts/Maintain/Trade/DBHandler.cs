using Proyecto26;
using System.Collections.Generic;
using UnityEngine;

namespace NukeFactory
{
    public class DBHandler : MonoBehaviour
    {
        [Header("Item")]
        public string itemName;
        public string itemId;
        public string tradeId;

        [Header("Auction Items")]
        public List<PostInfo> auctionItemsList = new List<PostInfo>();

        [System.Serializable]
        public class PostInfo
        {
            public string itemName;
            public string itemId;
            public string tradeId;
        }

        [ContextMenu("Send Item")]
        public void SendItem()
        {
            SetToDB(CreateItem(itemName, itemId, tradeId));
        }

        [ContextMenu("Get Item")]
        public void GetItem()
        {
            GetFromDB(itemName, itemId);
        }

        [ContextMenu("Delete Item")]
        public void DeleteItem()
        {
            DeleteFromDB(itemName, itemId);
        }

        [ContextMenu("Get Items by Name")]
        public void GetAllItemsByName()
        {
            GetFromDB(itemName);
        }

        private PostInfo CreateItem(string ItemName, string ItemId, string TradeId)
        {
            var item = new PostInfo();
            item.itemName = ItemName;
            item.itemId = ItemId;
            item.tradeId = TradeId;
            return item;
        }

        #region DB Handlers

        private void SetToDB(PostInfo post)
        {
            RestClient.Put(CreateURL(post), post).Then(response =>
            {
                Debug.Log("Put " + post.itemName + "(" + post.itemId + ") to DB");

            }).Catch(error =>
            {
                Debug.LogError("Failed to put " + post.itemName + "(" + post.itemId + ") to "
                    + CreateURL(post) + " | Error: " + error.Message);
            });
        }

        private void GetFromDB(string itemName, string itemId)
        {
            RestClient.Get<PostInfo>(CreateReadonlyURL(itemName, itemId)).Then(response =>
            {
                Debug.Log("Get " + itemName + "(" + itemId + ") from DB");
                auctionItemsList.Add(response);

            }).Catch(error =>
            {
                Debug.LogError("Failed to get " + itemName + "(" + itemId + ") from "
                    + CreateReadonlyURL(itemName, itemId) + " | Error: " + error.Message);
            });
        }

        private void GetFromDB(string itemName)
        {
            RestClient.Get(CreateReadonlyURL(itemName)).Then(response =>
            {
                Debug.Log("Deserialize query: " + response.Text);


            }).Catch(error =>
            {
                Debug.LogError("Failed to get " + itemName + "(" + itemId + ") from "
                    + CreateReadonlyURL(itemName, itemId) + " | Error: " + error.Message);
            });
        }

        private void DeleteFromDB(string itemName, string itemId)
        {
            RestClient.Delete(CreateReadonlyURL(itemName, itemId)).Then(response =>
            {
                Debug.Log("Delete " + itemName + "(" + itemId + ") from DB");

            }).Catch(error =>
            {
                Debug.LogError("Failed to delete " + itemName + "(" + itemId + ") from "
                    + CreateReadonlyURL(itemName, itemId) + " | Error: " + error.Message);
            });
        }

        #endregion

        #region URL

        [Header("Database")]
        //public DatabaseAuth databaseAuth;
        string serverUrl = "https://playstel-default-rtdb.europe-west1.firebasedatabase.app/";
        string postFormat = ".json";
        string postAuth = "?auth=";
        public bool secureToken;

        private string CreateURL(PostInfo post)
        {
            return serverUrl + post.itemName + "/" + post.itemId + postFormat;

        }

        private string CreateReadonlyURL(string ItemName, string ItemId)
        {
            return serverUrl + ItemName + "/" + ItemId + postFormat;

        }

        private string CreateReadonlyURL(string ItemName)
        {
            return serverUrl + ItemName + postFormat;

        }

        #endregion

    }
}

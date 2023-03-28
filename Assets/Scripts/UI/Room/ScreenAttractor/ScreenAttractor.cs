using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenAttractor : MonoBehaviour
{
    public static ScreenAttractor attractor;

    public Camera mainCamera;
    public List<ItemDisplay> items = new List<ItemDisplay>();

    [System.Serializable]
    public class ItemDisplay 
    {
        public string itemName;
        public GameObject iconPrefab;
        public TextMeshProUGUI countDisplay;
        public Animation crushAnim;

        [HideInInspector] public Vector3 attractorPos;
        [HideInInspector] public int itemsCount;

        public void SavePos()
        {
            attractorPos = crushAnim.transform.position;
        }

        public void UpdateItemValue(int amount)
        {
            itemsCount = amount;
            UpdateDisplayCount();
        }

        public void ReceiveItem()
        {
            itemsCount++;
            UpdateDisplayCount();
            crushAnim.Play();

            if (itemName == "Coin")
            {
                //if (CoinKeeper.coins) CoinKeeper.coins.AddCoin();
            }
        }

        public void RemoveItem(int amount)
        {
            itemsCount -= amount;
            UpdateDisplayCount();
            crushAnim.Play();

            if (itemName == "Coin")
            {
                //if (CoinKeeper.coins) CoinKeeper.coins.RemoveCoins(amount);
            }
        }

        public void UpdateDisplayCount()
        {
            countDisplay.text = itemsCount.ToString();
        }
    }

    #region Singleton

    public void Awake()
    {
        if (!attractor)
        {
            attractor = this;
        }
        else
        {
            if (attractor == this)
            {
                Debug.Log("Destroy Screen Attractor");
                Destroy(attractor.gameObject);
                attractor = this;
            }
        }

        SaveAttractorPos();
    }

    #endregion

    public void Start()
    {
        UpdateValues();
    }

    #region Handler

    private void UpdateValues()
    {
        // if (CoinKeeper.coins)
        // {
        //     if (GetItemDisplay("Coin") != null)
        //     {
        //         var characterCoins = CoinKeeper.coins.characterCoins;
        //         GetItemDisplay("Coin").UpdateItemValue(characterCoins);
        //     }
        // }
    }

    private void SaveAttractorPos()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].SavePos();
    }

    private ItemDisplay GetItemDisplay(string itemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
            {
                return items[i];
            }
        }

        Debug.Log("Failed to find ItemDisplay for " + itemName);

        return null;
    }

    #endregion

    #region Listener

    public void ReceiveItem(string itemName)
    {
        GetItemDisplay(itemName).ReceiveItem();
    }

    public void RemoveWoodenKey()
    {
        RemoveItem("WoodenKey", 1);
    }

    public void RemoveMetalKey()
    {
        RemoveItem("MetalKey", 1);
    }

    public void RemoveItem(string itemName, int amount)
    {
        GetItemDisplay(itemName).RemoveItem(amount);
    }

    public void NoMoreCoins()
    {
        GetItemDisplay("Coin").crushAnim.Play();
    }

    public Vector3 GetAttractorPos(string itemName)
    {
        return GetItemDisplay(itemName).attractorPos;
    }

    #endregion

    #region Icon Spawner

    public void CreateAttractableItem(Transform itemTransform, string itemName)
    {
        //AudioHandler.audioHandler.SoundCoin();

        var iconPrefab = GetItemDisplay(itemName).iconPrefab;

        GameObject newItem = Instantiate(iconPrefab, transform.parent);
        var guiTransform = GuiTransform(itemTransform);
        newItem.GetComponent<ScreenAttractableItem>().StartAttraction(guiTransform);
    }

    private Vector3 GuiTransform(Transform worldTransform)
    {
        return mainCamera.WorldToScreenPoint(worldTransform.position);
    }

    #endregion

}

using System;
using System.Collections.Generic;
using Playstel;
using TMPro;
using UnityEngine;

public class ShopItemStat : MonoBehaviour
{
    public ItemStat.StatKey StatKey;
    public ItemInfo.DataType DataType;
    public TextMeshProUGUI statValueText;

    public void SetValue(ItemInfo info)
    {
        var customData = info.GetTypedData(DataType);

        if (customData == null)
        {
            Debug.Log(info.itemName + " doesn't contains Stat key"); 
            gameObject.SetActive(false);
            return;
        }

        if (StringValue(customData)) return;
        
        var statValue = DataHandler.GetUnsafeValueInt(customData, StatKey.ToString());

        if (statValue == 0)
        {
            if (info.itemCatalog == ItemInfo.Catalog.Weapons)
            {
                statValueText.text = "-";
            }
            
            if (info.itemCatalog == ItemInfo.Catalog.Support)
            {
                gameObject.SetActive(false);
            }

            return;
        } 

        if (StatKey == ItemStat.StatKey.ReloadTime)
        {
            statValueText.text = Round(statValue);
            return;
        }
        
        if (StatKey == ItemStat.StatKey.ViewRange)
        {
            statValueText.text = Round(statValue, 1);
            return;
        }
        
        if (StatKey == ItemStat.StatKey.AttackRate)
        {
            statValueText.text = Round(statValue, 3, 1000);
            return;
        }
        
        if (StatKey == ItemStat.StatKey.Weight)
        {
            statValueText.text = Round(statValue);
            return;
        }

        if (StatKey == ItemStat.StatKey.Speed)
        {
            statValueText.text = GetPrefix() + Round(statValue);
            return;
        }

        statValueText.text = GetPrefix() + statValue + GetSuffix();
    }

    private string Round(int statValue, int digits = 2, int divider = 100)
    {
        var reloadTime = (float) statValue / divider;
        var reloadTimeRounded = Math.Round(reloadTime, digits);
        return reloadTimeRounded.ToString();
    }

    private bool StringValue(Dictionary<string, string> customData)
    {
        if (DataType == ItemInfo.DataType.StatString)
        {
            var statValueString = DataHandler.GetUnsafeValue(customData, StatKey.ToString());

            if (string.IsNullOrEmpty(statValueString))
            {
                if (StatKey == ItemStat.StatKey.Impact)
                {
                    statValueText.text = "Common";
                    return true;
                }

                gameObject.SetActive(false);
                return true;
            }

            statValueText.text = statValueString;

            return true;
        }

        return false;
    }

    private string GetSuffix()
    {
        var suffix = "";
        if (StatKey == ItemStat.StatKey.Accuracy) suffix = "/30";
        return suffix;
    }
    
    private string GetPrefix()
    {
        var prefix = "";
        if (StatKey == ItemStat.StatKey.Health) prefix = "+";
        if (StatKey == ItemStat.StatKey.Speed) prefix = "x";
        if (StatKey == ItemStat.StatKey.Holders) prefix = "+";
        return prefix;
    }
}

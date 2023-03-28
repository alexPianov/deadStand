using System.Collections.Generic;
using Playstel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBundleSlot : MonoBehaviour
{
    [Header("Level")]
    public TextMeshProUGUI levelNumber;
    public Image imageStar;
    public Transform focusPlaceholder;

    [Header("Bundle Items")]
    public UiProgressBundleItem commonItem;
    public UiProgressBundleItem premiumItem;

    public Image pathLeft;
    public Image pathRight;
    
    private int _levelNumber;
    public List<Item> _bundleItems;

    public void SetBundleItems(List<Item> progressBundle)
    {
        _bundleItems = progressBundle;
    }

    public void UpdateBundleItems(bool battlePass, bool isLocked)
    {
        imageStar.enabled = !isLocked;
        
        foreach (var item in _bundleItems)
        {
            if (item.info.IsPremium())
            {
                premiumItem.SetItem(item, isLocked);
                premiumItem.BattlePassLock(battlePass, isLocked);
            }
            else
            {
                commonItem.SetItem(item, isLocked);
            }
        }
    }

    public void SetBundleLevel(int number, int maxNumber)
    {
        _levelNumber = number;
        levelNumber.text = number.ToString();

        pathLeft.enabled = number != 1;
        pathRight.enabled = number != maxNumber;
    }

    public int GetLevelNumber()
    {
        return _levelNumber;
    }

    public List<Item> GetBundleItems()
    {
        return _bundleItems;
    }

    public void SetFocus(Transform focus)
    {
        focus.SetParent(focusPlaceholder);
        focus.localPosition = new Vector3(0, 0, 0);
    }

    public void SetBundleItemDemo(UiProgressItemShow itemDemo)
    {
        foreach (var bundleItem in _bundleItems)
        {
            if (bundleItem.info.IsPremium())
            {
                premiumItem.SetItemDemo(itemDemo);
            }
            else
            {
                commonItem.SetItemDemo(itemDemo);
            }
        }
    }
}

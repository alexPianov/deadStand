using Playstel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button), typeof(SlotRenderer))]
public class UiCustomizeUnitSlot : MonoBehaviour
{
    public Image Image;
    public GameObject IconNew;

    [Inject] private CacheUserInfo _cacheUserInfo;
    [Inject] private Unit _unit;
    
    private ItemInfo _currentItemInfo;
    private SlotRenderer _slotRenderer;
    private Transform _focus;

    private void Awake()
    {
        _slotRenderer = GetComponent<SlotRenderer>();
        GetComponent<Button>().onClick.AddListener(Pick);
    }

    public void SetFocus(Transform focus)
    {
        _focus = focus;
    }

    public void SetItemInfo(ItemInfo itemInfo)
    {
        _currentItemInfo = itemInfo;
        
        SetSprite(_currentItemInfo);
        CheckPickedItemFocus();
        DisableTransparency();
    }
    
    private void DisableTransparency()
    {
        _slotRenderer.Transparency(false);
    }

    private void SetSprite(ItemInfo itemInfo)
    {
        Image.sprite = itemInfo.itemSprite;
    }

    private void CheckPickedItemFocus()
    {
        var result = _unit.Items.GetSameItems(_currentItemInfo.itemName);

        if (result != null && result.Count > 0)
        {
            transform.SetSiblingIndex(0);
            SetFocus();
        }
    }

    private void Pick()
    {
        var result = _unit.Builder.IsAlreadyAdd(_currentItemInfo);

        if (result)
        {
            CancelFocus();
            return;
        }

        _unit.Builder.AddItem(_currentItemInfo);
        SetFocus();
        
        _cacheUserInfo.inventory.ExcludeFormGrantedItems(_currentItemInfo);
        MarkAsJustGranted(false);
    }
    
    public void MarkAsJustGranted(bool state)
    {
        IconNew.SetActive(state);
    }

    private void SetFocus()
    {
        _focus.SetParent(transform);
        _focus.localPosition = new Vector3(0, 0, 0);
    }

    private void CancelFocus()
    {
        _focus.SetParent(null);
        _focus.localPosition = new Vector3(0, 3000, 0);
    }
}

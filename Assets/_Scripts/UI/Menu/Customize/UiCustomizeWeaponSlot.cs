using Playstel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UiCustomizeWeaponSlot : MonoBehaviour
{
    public Image Image;
    public Image IconNew;

    private GameObject _weaponInstance;
    private ItemInfo _currentItemInfo;
    
    [Inject] private CacheUserInfo _cacheUserInfo;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Pick);
    }

    public void BindInstance(GameObject instance)
    {
        _weaponInstance = instance;
    }
    
    public void MarkAsJustGranted(bool state)
    {
        IconNew.enabled = state;
    }

    public void SetItem(ItemInfo infoInfo)
    {
        _currentItemInfo = infoInfo;
        Image.sprite = infoInfo.itemSprite;
    }

    private async void Pick()
    {
        var materialName = _currentItemInfo
            .GetUnsafeValue(ItemInfo.DataType.Material.ToString());
        
        var material = await AddressablesHandler.Load<Material>(materialName);
        
        _cacheUserInfo.inventory.ExcludeFormGrantedItems(_currentItemInfo);
        _weaponInstance.GetComponent<ItemRenderer>().SetMaterial(material);
    }
}

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class NpcCharacter : MonoBehaviour
    {
        private NpcCharacterDialog _npcCharacterDialog;
        public ItemInfo _itemInfo;

        private Unit _unit;
        
        private void Awake()
        {
            _npcCharacterDialog = GetComponent<NpcCharacterDialog>();
            
            _unit = GetComponent<Unit>();
        }

        public async UniTask Initialize(ItemInfo info, NpcCharacterProfile profile, bool activeGizmos = true)
        {
            _itemInfo = info;

            if (_unit.Gizmo)
            {
                _unit.Gizmo.SetNpcGizmo(GetCharacterName());
                _unit.Gizmo.ActiveNpcGizmo(activeGizmos);
            }
            
            if(_npcCharacterDialog) _npcCharacterDialog.SetDialogProfile(profile);
            
            var schemeItems = GetCharacterItems(info);
            await _unit.Builder.BuildUnit(schemeItems);
        }

        public string GetCharacterName()
        {
            return _itemInfo.itemName;
        }

        public enum DataKey
        {
            DialogButtons, Weapon, Collects, ItemsForTrade
        }
        
        public string GetCharacterData(DataKey key)
        {
            var data = _itemInfo.GetUnsafeValue(key.ToString());
            
            if (string.IsNullOrEmpty(data))
            {
                Debug.Log("Failed to find key " + key + " in " + _itemInfo.itemName);
                return null;
            }

            return data;
        }

        private string[] GetCharacterItems(ItemInfo info)
        {
            var items = new List<string>();
            
            items.AddRange(info.GetCatalogItem().Bundle.BundledItems);
            
            var weaponName = info.GetUnsafeValue(DataKey.Weapon.ToString());
            
            string unitItemNames = null;

            foreach (var itemName in items)
            {
                unitItemNames += itemName + ";";
            }
            
            if (weaponName != null) items.Add(weaponName);

            return items.ToArray();
        }
    }
}

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playstel
{
    [RequireComponent(typeof(UiCharacterPick))]
    public class UiCharacterSlotList : UiFactory
    {
        public Transform focus;
        public bool randomChooseAtStart;
        
        private List<UiCharacterSlot> _characterSlots = new ();

        private UiCharacterPick _characterActivator;
        private void Awake()
        {
            _characterActivator = GetComponent<UiCharacterPick>();
        }

        private async void Start()
        {
            await SetCharacterBundles();

            if (randomChooseAtStart)
            {
                ActiveRandomBundle();
            }
            else
            {
                SetPickedCharacterSetup();
            }
        }

        private async UniTask SetCharacterBundles()
        {
            var characterBundles = GetItemsFromSource(ItemSource.External,
                ItemInfo.Catalog.Character, ItemInfo.Class.Scheme, ItemInfo.Subclass.Player);

            await CreateBundleSlots(characterBundles);
        }

        private async UniTask CreateBundleSlots(List<Item> characterBundles)
        {
            foreach (var bundle in characterBundles)
            {
                var instance = await CreateSlot(SlotName.CharacterSlot);
                var characterSlot = instance.GetComponent<UiCharacterSlot>();
                
                characterSlot.SetCharacterInfo(bundle.info);
                characterSlot.SetFocusTransform(focus);
                characterSlot.SetCharacterActivator(_characterActivator);
                
                _characterSlots.Add(characterSlot);
            }
        }

        private void ActiveRandomBundle()
        {
            var randomSlot = _characterSlots[Random.Range(0, _characterSlots.Count - 1)];
            randomSlot.transform.SetSiblingIndex(0);
            randomSlot.Active();
        }

        private void SetPickedCharacterSetup()
        {
            if (GetComponent<UiCharacterLoad>())
            {
                GetComponent<UiCharacterLoad>().LoadCharacterComponents(_characterSlots);
            }
        }
    }   
}

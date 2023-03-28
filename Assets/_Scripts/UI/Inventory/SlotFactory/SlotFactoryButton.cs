using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    [RequireComponent(typeof(Button))]
    public class SlotFactoryButton : MonoBehaviour
    {
        [Header("Setup")]
        public ItemInfo.Catalog itemCatalog;
        public ItemInfo.Class itemClass;
        public ItemInfo.Subclass itemSubclass;
        private SlotFactoryButtonGroup _buttonGroup;
        
        public void InitButtonGroup(SlotFactoryButtonGroup buttonGroup)
        {
            _buttonGroup = buttonGroup;
        }

        public void Start()
        {
            GetComponent<Button>().onClick.AddListener(Active);
        }

        public void Active()
        {
            _buttonGroup.slotFactory.CreateSlots(itemCatalog, itemClass, itemSubclass);
            _buttonGroup.focusLine.transform.SetParent(transform, false);
        }
    }
}
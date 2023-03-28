using UnityEngine;

namespace Playstel
{
    public class SlotFactoryUnit : SlotFactory
    {
        [Header("Default")] 
        public ItemInfo.Catalog defaultCatalog = ItemInfo.Catalog.Backpack;
        public ItemInfo.Class defaultClass = ItemInfo.Class.Loot;
        public ItemInfo.Subclass defaultSubclass = ItemInfo.Subclass.Null;

        public void Start()
        {
            CreateSlotTriggers(GetUnit().Items.GetMaxBagSlots());
            CreateSlots(defaultCatalog, defaultClass, defaultSubclass);
        }
    }
}

using UnityEngine;

namespace Playstel
{
    public class SlotFactoryUnitSaveLoot : SlotFactoryUnit
    {
        private const ItemInfo.Catalog lootCatalog = ItemInfo.Catalog.Backpack;
        
        public void OnDisable()
        {
            TakeLoot();
        }

        private async void TakeLoot()
        {
            if(!GetUnit().photonView.IsMine) return;

            if(GetUnit().Death.IsDead()) return;
            
            var takedItems = GetFactoryItemList();

            var request = HandlerHostRequest
                .GetLootRequest(takedItems, lootCatalog);

            await GetUnit().HostOperator
                .Run(UnitHostOperator.Operation.Loot, request);
        }
    }
}

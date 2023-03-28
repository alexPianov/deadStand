using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UICrateHandle : MonoBehaviour
    {
        public SlotFactory SlotFactory;
        
        [Inject]
        private Unit _unit;

        private Crate _crate;

        public async void Start()
        {
            await UniTask.WaitUntil(() => _unit.Interaction.Crate);

            _crate = _unit.Interaction.Crate;
            
            await SlotFactory.CreateSlotTriggers(_crate.Handler.GetCrateSize());
            await SlotFactory.CreateSlots(_crate.Items.Get());
        }

        public void OnDisable()
        {
            _crate.Items.UpdateItems(SlotFactory.GetFactoryItemList());
        }
    }
}


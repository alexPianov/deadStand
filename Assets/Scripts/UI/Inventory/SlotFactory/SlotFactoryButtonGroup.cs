using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playstel
{
    public class SlotFactoryButtonGroup : MonoBehaviour
    {
        [Header("Refs")]
        public SlotFactory slotFactory;

        public Transform focusLine;

        public List<SlotFactoryButton> slotFactoryButtonList = new ();

        public void Start()
        {
            InitFactoryButtons();
        }

        private void InitFactoryButtons()
        {
            slotFactoryButtonList = GetComponentsInChildren<SlotFactoryButton>().ToList();

            foreach (var factoryButton in slotFactoryButtonList)
            {
                factoryButton.InitButtonGroup(this);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class UnitRoot : MonoBehaviour
    {
        [Header("Refs")]
        public Transform head;
        public Transform primary;
        public Transform secondary;
        public Transform backpack;
        public Transform leftHand;
        public Transform rightHand;

        public Transform GetRootPart(ItemInfo info)
        {
            if (info.itemClass == ItemInfo.Class.Firearm)
            {
                return primary;
            }

            if (info.itemClass == ItemInfo.Class.Melee)
            {
                return secondary;
            }

            if (info.itemClass == ItemInfo.Class.Grenade)
            {
                return rightHand;
            }

            if (info.itemCatalog == ItemInfo.Catalog.Character)
            {
                if (info.ItemSubclass == ItemInfo.Subclass.Rucksack)
                {
                    return backpack;
                }

                return head;
            }

            return null;
        }
    }
}

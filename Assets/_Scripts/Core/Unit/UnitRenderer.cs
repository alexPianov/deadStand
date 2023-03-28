using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class UnitRenderer : MonoBehaviour
    {
        public bool renderIsActive;
        
        public void Start()
        {
            Active(true);
        }

        public void Active(bool state)
        {
            renderIsActive = state;
            
            if(!this) return;

            if (TryGetComponent(out UnitSkin unitSkin))
            {
                unitSkin.Active(state);
            }
            
            if (TryGetComponent(out NpcCharacterSkin npcSkin))
            {
                npcSkin.Active(state);
            }

            if (TryGetComponent(out UnitHandleItems handleItems))
            {
                var items = handleItems.GetHandleInstanceList();

                for (int i = 0; i < items.Count; i++)
                {
                    if (!items[i]) continue;
                    items[i].GetComponent<ItemRenderer>().Active(state);
                }
            }
        }

        public void SetLayer(int number)
        {
            GetComponent<UnitSkin>().SetLayer(number);

            if (!GetComponent<UnitHandleItems>()) return;

            var items = GetComponent<UnitHandleItems>().GetHandleInstanceList();

            for (int i = 0; i < items.Count; i++)
            {
                items[i].GetComponent<ItemRenderer>().SetLayers(number);
            }
        }
    }
}

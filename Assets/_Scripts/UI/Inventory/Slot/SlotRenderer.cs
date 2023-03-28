using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class SlotRenderer : UiTransparency
    {
        public Image selectOutline;

        private void Awake()
        {
            selectOutline.enabled = false;
        }

        public void Outline()
        {
            if(selectOutline) selectOutline.enabled = !selectOutline.enabled;
        }
        
        public void Outline(bool state)
        {
            if(selectOutline) selectOutline.enabled = state;
        }
    }
}

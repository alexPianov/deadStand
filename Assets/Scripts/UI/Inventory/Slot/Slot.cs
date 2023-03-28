using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Playstel
{
    [RequireComponent(typeof(SlotDisplay), typeof(SlotTween), typeof(SlotRenderer))]
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public SlotDisplay display;
        [HideInInspector] public SlotTween tween;
        [HideInInspector] public SlotRenderer renderer;
        [HideInInspector] public SlotDrag drag;

        [Header("Item")]
        public Item currentItem;
        public SlotFactory currentFactory;
        
        public void Awake()
        {
            display = GetComponent<SlotDisplay>();
            tween = GetComponent<SlotTween>();
            renderer = GetComponent<SlotRenderer>();
            drag = GetComponent<SlotDrag>();
        }

        public void InitSlot(Item item)
        {
            currentItem = item;
            name = "Slot_" + currentItem.info.itemName;
            
            display.SetItem(item);
            renderer.Transparency(false);
            tween.Punch();
        }

        public void SetSlotFactory(SlotFactory factory)
        {
            currentFactory = factory;
        }

        public async UniTask RemoveSlot()
        {
            renderer.Transparency(true);
            await tween.RemoveTween();
            
            if (gameObject) Destroy(gameObject);
        }

        public void DeselectSlot()
        {
            currentFactory.GetFactorySelect().DeselectSlot(this);
        }

        public void SelectSlot()
        {
            currentFactory.GetFactorySelect().ReceiveSlot(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (drag.IsDraging()) return;
            SelectSlot();
        }
    }
}

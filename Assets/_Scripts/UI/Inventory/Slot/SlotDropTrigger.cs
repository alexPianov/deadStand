using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class SlotDropTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        [Header("Cache")]
        public SlotFactory SlotFactory;
        public Slot CurrentSlot;
        
        private SlotRenderer _triggerRenderer;
        private Transform _transform;
        
        public void Awake()
        {
            _transform = transform;
            _triggerRenderer = GetComponent<SlotRenderer>();
            _triggerRenderer.Transparency(false);
        }

        public void SetFactory(SlotFactory factory)
        {
            SlotFactory = factory;
        }
        
        public async UniTask AttachSlot(Slot slot)
        {
            await MoveSlotToTrigger(slot);
            
            CurrentSlot = slot;
            BindSlotAsChild(slot);
            
            slot.DeselectSlot();
            slot.drag.SetNewTrigger(null);
            slot.SetSlotFactory(SlotFactory);

            slot.renderer.BlocksRaycasts(true);

            IsVisible(false);
            Outline(false);
        }

        public void DetachSlot()
        {
            CurrentSlot.renderer.BlocksRaycasts(false);
            IsVisible(true);
            CurrentSlot = null;
        }

        private async Task MoveSlotToTrigger(Slot slot)
        {
            await slot.tween.Move(_transform);
        }

        private void BindSlotAsChild(Slot slot)
        {
            slot.transform.SetParent(_transform);
            slot.transform.position = new Vector3(0, 0, 0);
            slot.transform.localPosition = new Vector3(0, 0, 0);
            slot.drag.dropTriggerDefault = this;
        }
        
        public void Outline(bool state)
        {
            _triggerRenderer.Outline(state);
        }
        
        private void IsVisible(bool state)
        {
            _triggerRenderer.Enable(state);
        }

        public async void OnDrop(PointerEventData eventData)
        {
            var slot = eventData.pointerDrag.GetComponent<Slot>();
        
            if (CurrentSlot)
            {
                return;
            }
            
            if (slot)
            {
                await AttachSlot(slot);
                slot.drag.dropTriggerDefault.Outline(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            
            if (CurrentSlot)
            {
                return;
            }
            
            var slotDrag = eventData.pointerDrag.GetComponent<SlotDrag>();

            if (slotDrag)
            {           
                slotDrag.SetNewTrigger(this);
                _triggerRenderer.Outline(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            
            if (CurrentSlot)
            {
                return;
            }

            var slotDrag = eventData.pointerDrag.GetComponent<SlotDrag>();

            if (slotDrag)
            {
                slotDrag.SetNewTrigger(null);
                _triggerRenderer.Outline(false);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Playstel
{
    public class SlotDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Parents")] 
        public SlotDropTrigger dropTriggerDefault;
        public SlotDropTrigger dropTriggerNew;

        [Header("Cache")] 
        private Slot _slot;
        private Transform _transform, _transformCanvas;
        private Vector2 _offset;
        private SlotDragCanvas _slotDragCanvas;
        
        [Inject] private CacheAudio _cacheAudio;

        private void Awake()
        {
            _slot = GetComponent<Slot>();
        }
        
        [Inject]
        private void Construct(SlotDragCanvas slotInstaller)
        {
            _slotDragCanvas = slotInstaller;
            _transformCanvas = _slotDragCanvas.storageCanvas;
            _transform = transform;
        }

        private bool _onBeingDrag;
        private float dragScale = 1.15f;

        public bool IsDraging()
        {
            return _onBeingDrag;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _cacheAudio.Play(CacheAudio.MenuSound.OnProcess);
            
            ReleaseFromTrigger();
            SetOffset(eventData);
            
            _onBeingDrag = true;
        }

        public void ReleaseFromTrigger()
        {
            dropTriggerDefault = GetDropTrigger();
            dropTriggerDefault.DetachSlot();
            _transform.SetParent(_transformCanvas);
        }

        private void SetOffset(PointerEventData eventData)
        {
            var pos = _transform.position;
            _offset = new Vector2(pos.x, pos.y) - eventData.position;
        }

        private SlotDropTrigger GetDropTrigger()
        {
            return _transform.parent.GetComponent<SlotDropTrigger>();
        }

        public void SetNewTrigger(SlotDropTrigger triggerTransform)
        {
            dropTriggerNew = triggerTransform;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var newPos = eventData.position;
            _transform.position = newPos + _offset;
        }

        public async void OnEndDrag(PointerEventData eventData)
        {
            _onBeingDrag = false;
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnToggle);

            if (dropTriggerNew == null)
            {
                await dropTriggerDefault.AttachSlot(_slot);
            }
        }
    }
}

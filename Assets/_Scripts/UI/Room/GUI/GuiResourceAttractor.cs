using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class GuiResourceAttractor : MonoBehaviour, IAttractorUi
    {
        public Image reference;
        public GameObject attractionSlot;

        [Inject] private Unit _unit;
        
        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        public async void HandlerAttract(Transform target)
        {
            Debug.Log("HandlerAttract");
            
            GameObject attractionObject = Instantiate(attractionSlot, transform.parent);
            
            if (attractionObject.TryGetComponent(out Image image))
            {
                image.sprite = reference.sprite;
                image.CrossFadeAlpha(0,0,true);
                image.CrossFadeAlpha(1,0.2f,true);
            }
            
            attractionObject.transform.position = GuiTransform(target);
            
            await Move(attractionObject.transform, transform);
            
            image.CrossFadeAlpha(0, 0.2f,true);

            await UniTask.Delay(200);

            Destroy(attractionObject);
        }
        
        private Vector3 GuiTransform(Transform worldTransform)
        {
            return _unit.Camera.GetCameraObserver().MainCamera.WorldToScreenPoint(worldTransform.position);
        }
        
        public async UniTask Move(Transform _item, Transform _target)
        {
            if(!_target) { Debug.Log("_transformTarget is null for " + name); return; }
            
            await _item.DOMove(_target.position, 0.7f, true)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();
        }
    }
}
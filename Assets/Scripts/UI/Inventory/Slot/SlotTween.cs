using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Playstel
{
    public class SlotTween : MonoBehaviour
    {
        private Transform _transform;

        public void Start()
        {
            _transform = transform;
        }

        public async UniTask RemoveTween(float duration = 0.1f)
        {
            ShakeUp();

            await _transform.DOMove(_transform.position + Vector3.up * 150, duration, true)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();
        }

        public async UniTask Punch(float power = 150)
        {
            await _transform.DOPunchPosition(Vector3.up * power, 0.15f)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();

            //ShakeUp();
        }

        public async UniTask Move(Transform _transformTarget)
        {
            if(!_transformTarget) { Debug.Log("_transformTarget is null for " + name); return; }
            
            await _transform.DOMove(_transformTarget.position, 0.10f, true)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();
        }

        public async UniTask Scale(Transform _transform, float endValue = 1f, float duration = 0.2f)
        {
            await _transform.DOScale(endValue, duration).AsyncWaitForCompletion();
        }

        public void ShakeUp(float duration = 0.2f, float strength = 20f)
        {
            _transform.DOShakePosition(duration, Vector3.up * strength);
            _transform.DOShakeRotation(duration, Vector3.up * strength);
        }

        public void Shake(float duration = 0.2f)
        {
            _transform.DOShakeScale(duration, 2);
        }
    }
}

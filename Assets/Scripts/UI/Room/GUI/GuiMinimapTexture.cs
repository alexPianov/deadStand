using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class GuiMinimapTexture : MonoBehaviour
    {
        public RawImage RawImage;

        [Inject]
        private Unit _unit;

        public async void Start()
        {
            await UniTask.WaitUntil(() => _unit);
            
            await UniTask.WaitUntil(() => _unit.Camera);
            
            await UniTask.WaitUntil(() => _unit.Camera.GetCameraObserver());
            
            await UniTask.WaitUntil(() => _unit.Camera.GetCameraObserver().CameraMinimapRender);
            
            await UniTask.WaitUntil(() => _unit.Camera.GetCameraObserver()
                .CameraMinimapRender.GetRenderTexture());
            
            RawImage.texture = _unit.Camera.GetCameraObserver().CameraMinimapRender.GetRenderTexture();
        }
    }
}
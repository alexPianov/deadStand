using Cysharp.Threading.Tasks;
using Playstel;
using UnityEngine;

public class CameraMinimapRender : MonoBehaviour
{
    private const string minimapTexture = "MinimapRenderTexture";
    public Camera minimapCamera;
    public RenderTexture _renderTexture;
    private const int defaultSize = 40;
    
    public async UniTask CreateRenderTexture()
    {
        _renderTexture = await AddressablesHandler.Load<RenderTexture>(minimapTexture);
        minimapCamera.targetTexture = _renderTexture;
    }

    public RenderTexture GetRenderTexture()
    {
        return _renderTexture;
    }

    public void SetOrtographicSize(float size = defaultSize)
    {
        minimapCamera.orthographicSize = size;
    }
}

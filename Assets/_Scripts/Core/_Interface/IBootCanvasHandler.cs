using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public interface IBootCanvasHandler : IGlobalSubscriber
    {
        void HandleCanvasTransparency(bool canvasTransparency);
    }
}
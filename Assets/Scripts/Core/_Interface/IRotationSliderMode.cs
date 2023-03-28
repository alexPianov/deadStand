using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public interface IRotationSliderMode : IGlobalSubscriber
    {
        void HandlerActiveRotationSlider(bool state);
    }
}
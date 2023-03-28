using EventBusSystem;

namespace Playstel
{
    public interface IGizmosRotateHandler : IGlobalSubscriber
    {
        void HandleCameraRotationChange(float value);
    }
}
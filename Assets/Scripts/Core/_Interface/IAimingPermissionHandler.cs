using EventBusSystem;

namespace Playstel
{
    public interface IAimingPermissionHandler : IGlobalSubscriber
    {
        void HandlerAimingPermission(bool state);
    }
}
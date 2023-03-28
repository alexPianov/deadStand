using EventBusSystem;

namespace Playstel
{
    public interface IAnnounceHandler : IGlobalSubscriber
    {
        void HandleValue(string announce, bool skipQueue = false);
    }
}
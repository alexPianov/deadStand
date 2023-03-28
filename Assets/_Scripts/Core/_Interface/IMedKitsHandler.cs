using EventBusSystem;

namespace Playstel
{
    public interface IMedKitsHandler : IGlobalSubscriber
    {
        void HandleValue(int amount);
    }
}

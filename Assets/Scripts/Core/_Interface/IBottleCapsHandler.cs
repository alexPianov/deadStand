using EventBusSystem;

namespace Playstel
{
    public interface IBottleCapsHandler : IGlobalSubscriber
    {
        void HandleValue(int amount);
    }
}

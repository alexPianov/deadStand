using EventBusSystem;

namespace Playstel
{
    public interface ITokenHandler : IGlobalSubscriber
    {
        void HandleValue(int amount);
    }
}

using EventBusSystem;

namespace Playstel
{
    public interface IWaitHandler : IGlobalSubscriber
    {
        void HandleMaxWaitTime(float maxWaitTime);
        void HandleWaitTime(float waitTime);
    }
}

using EventBusSystem;

namespace Playstel
{
    public interface IAttackHandler : IGlobalSubscriber
    {
        void HandleAttackMode(bool state);

        void HandleAttackSliderValue(float rate);
    }
}

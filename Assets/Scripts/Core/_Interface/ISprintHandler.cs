using EventBusSystem;

namespace Playstel
{
    public interface ISprintHandler : IGlobalSubscriber
    {
        void HandleSprintMaxValue(int maxValue);
        void HandleSprintChange(float sprintValue);
    }
}

using EventBusSystem;

namespace Playstel
{
    public interface IExperienceHandler : IGlobalSubscriber
    {
        void HandleExperienceChange(int experienceAmount);
        void HandleMaxExperience(int experienceAmount);
        void HandleLevelChange(int level);
    }
}

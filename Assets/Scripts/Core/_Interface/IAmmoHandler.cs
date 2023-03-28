using EventBusSystem;

namespace Playstel
{
    public interface IAmmoHandler : IGlobalSubscriber
    {
        void HandleHolderChange(int holderRemain, int holdersCount);

        void HandleHolderReload(float reloadTime);
    }
}

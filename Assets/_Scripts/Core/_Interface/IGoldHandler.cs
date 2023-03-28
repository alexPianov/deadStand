using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public interface IGoldHandler : IGlobalSubscriber
    {
        void HandleValue(int amount);
    }
}
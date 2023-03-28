using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;

namespace Playstel
{
    public interface IAttractorUi : IGlobalSubscriber
    {
        void HandlerAttract(Transform target);
    }
}
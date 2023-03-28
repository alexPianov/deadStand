using UnityEngine;
using Zenject;

namespace Playstel
{
    public class SlotDragCanvas : MonoBehaviour
    {
        public Transform storageCanvas;

        [Inject]
        public void Construct(LocationInstaller locationInstaller)
        {
            locationInstaller.BindFromInstance(this);
        }
    }
}

using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiUnitRendererOnStart : UiUnitRendererActive
    {
        public bool activeOnStart;
        
        private void Awake()
        {
            ActiveRenderer(activeOnStart);
        }
    }
   
}
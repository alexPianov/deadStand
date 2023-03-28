using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UICrateDeathListener : MonoBehaviour
    {
        [Inject]
        private Unit _unit;
    
        private void Update()
        {
            if (!_unit) return;
            
            if (_unit.Death.IsDead())
            {
                if(TryGetComponent(out UIElementLoadButton elementLoadButton))
                {
                    elementLoadButton.Load();
                    return;
                }
                
                if(TryGetComponent(out UIElementLoadByType elementLoadByType))
                {
                    elementLoadByType.Load();
                }
            }
        }
    }
}

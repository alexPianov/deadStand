using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UnitOutline : MonoBehaviour
    {
        private Outline _outline;
        private Unit _unit;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public void CreateOutline(GameObject meshTransform)
        {
            if (!meshTransform)
            {
                Debug.Log("Failed to find mesh transform");
            }
            
            _outline = meshTransform.AddComponent<Outline>();
            time = 0;
        }

        public void ActiveOutline(UnitFraction.Fraction mineFraction)
        {
            activeOutline = true;
            time = 2f;
            SetOutlineColor(mineFraction);
        }

        private void SetOutlineColor(UnitFraction.Fraction mineFraction)
        {
            if(!_outline) return;
            
            if (!_unit.Fraction)
            {
                //_outline.OutlineColor = Color.white;
                return;
            }

            if (_unit.Fraction.currentFraction == mineFraction)
            {
                //_outline.OutlineColor = Color.white;
            }
            else
            {
                //_outline.OutlineColor = Color.red;
            }
        }

        private float time;
        private bool activeOutline;
        
        private void Update()
        {
            if (!activeOutline) return;
            if (!_outline) return;
            
            time -= Time.deltaTime;
            //_outline.OutlineWidth = time;
            
            if (time <= 0)
            {
                activeOutline = false;
                //_outline.OutlineWidth = 0;
            }
        }
    }
}

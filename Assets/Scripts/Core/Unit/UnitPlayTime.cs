using UnityEngine;

namespace Playstel
{
    public class UnitPlayTime : MonoBehaviour
    {
        private float _playSeconds;
        
        private void Update()
        {
            _playSeconds += Time.deltaTime;
        }

        public int GetMinutes()
        {
            return Mathf.FloorToInt(_playSeconds / 60);
        }
    }
}
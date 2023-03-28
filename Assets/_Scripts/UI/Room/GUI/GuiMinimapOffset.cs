using UnityEngine;
using Zenject;

namespace Playstel
{
    public class GuiMinimapOffset : MonoBehaviour
    { 
        private Transform _playerPosition;
        private Transform _currentPosition;
        private Vector3 _newPlayerPosition;
        
        private Vector3 _startPosition = new Vector3(0, 100, 0);
        private Vector3 _startRotation = new Vector3(90f, 160f, 0f);

        public void SetTargetToFollow(Transform unitTransform)
        {
            _currentPosition = transform;
            _playerPosition = unitTransform;
            
            _currentPosition.rotation = Quaternion.Euler(_startRotation);
            _currentPosition.position = _startPosition;
        }

        private void LateUpdate()
        {
            if(!_playerPosition) return;
            
            _newPlayerPosition = _playerPosition.position;
            _newPlayerPosition.y = _currentPosition.position.y;
            
            _currentPosition.position = _newPlayerPosition; 
        }
    }
}

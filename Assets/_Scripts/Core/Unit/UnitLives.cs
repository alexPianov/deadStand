using CodeStage.AntiCheat.ObscuredTypes;
using EventBusSystem;
using Photon.Pun;

namespace Playstel
{
    public class UnitLives : MonoBehaviourPun
    {
        public ObscuredInt _currentLives;
        public ObscuredInt _maxLives;

        public int GetLives()
        {
            return _currentLives;
        }

        public void SetMaxLives(int MaxLives)
        {
            _maxLives = MaxLives;
        }

        public int GetMaxLives()
        {
            return _maxLives;
        }

        public void UpdateLives(int value, bool updateUi = true)
        {
            _currentLives = value;
            if(updateUi) UpdateLivesUi();
        }

        public void UpdateLivesUi()
        {
            if (photonView.IsMine)
            {
                EventBus.RaiseEvent<IMedKitsHandler>
                    (h => h.HandleValue(_currentLives));
            }
        }
    }
}

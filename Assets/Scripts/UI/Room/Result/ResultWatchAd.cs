using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class ResultWatchAd : MonoBehaviour
    {
        public Button AdButton;
        
        private void Start()
        {
            AdButton.onClick.AddListener(WatchAd);
        }

        private void WatchAd()
        {
            Debug.Log("Watch Ad");
        }
    }
}
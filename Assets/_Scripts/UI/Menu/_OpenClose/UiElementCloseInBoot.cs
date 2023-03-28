using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playstel
{
    public class UiElementCloseInBoot: MonoBehaviour
    {
        public bool closeInMenu;
        public void OnEnable()
        {
            if (closeInMenu)
            {
                gameObject.SetActive(SceneManager.GetActiveScene().buildIndex == 0);
            }
            else
            {
                gameObject.SetActive(SceneManager.GetActiveScene().buildIndex != 0);
            }
        }
    }
}

using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playstel
{
    public static class SceneHandler 
    {
        public enum Scenes : int
        {
            Boot = 0,
            CreateCharacter = 1,
            Menu = 2
        }
        
        public static async UniTask LoadAsync(Scenes sceneIndex, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex == (int) sceneIndex)
            {
                Debug.Log("Scene is already loaded");
                return;
            }
            
            await SceneManager.LoadSceneAsync((int)sceneIndex, loadMode).AsAsyncOperationObservable();
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Install/Sprites")]
    public class InstallSprites : ScriptableObject
    {
        private IList<string> atlasList = new List<string>
        {
            "BackpackAtlas", "WeaponsAtlas", "SupportAtlas", "CharacterAtlas", "SetupAtlas"
        };

        public List<SpriteAtlas> GetSpriteAtlasList()
        {
            return _spriteAtlasList;
        }

        public async UniTask Install()
        {
            _spriteAtlasList.Clear();

            foreach (var atlas in atlasList)
            {
                await LoadAtlas(atlas);
            }
        }

        public async UniTask LoadAtlas(string spriteAtlasName)
        {
            var result = await Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasName);
            
            if (result)
            {
                SaveAtlas(result);
            }
        }

        private List<SpriteAtlas> _spriteAtlasList = new ();

        private Sprite[] _allSprites = { };
        private void SaveAtlas(SpriteAtlas atlas)
        {
            if (atlas == null)
            {
                Debug.LogError("Atlas is empty: " + atlas.name);
                return;
            }

            if (_spriteAtlasList.Contains(atlas))
            {
                Debug.LogError("Atlas handle is already exists: " + atlas.name);
                Addressables.Release(atlas);
                return;
            }

            _allSprites = new Sprite[atlas.spriteCount];

            atlas.GetSprites(_allSprites);

            foreach (Sprite sprite in _allSprites)
            {
                sprite.name = DataHandler.RemoveTextChars(sprite.name, "(Clone)");
            }

            _spriteAtlasList.Add(atlas);
        }
    }
}

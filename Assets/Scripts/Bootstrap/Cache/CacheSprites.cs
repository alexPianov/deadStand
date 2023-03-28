using UnityEngine;

namespace Playstel
{
    public class CacheSprites : MonoBehaviour
    {
        public InstallSprites install;

        private string atlasPrefix = "Atlas";
        public Sprite GetSpriteFromAtlas(string spriteName, ItemInfo.Catalog itemName)
        {
            var atlasList = install.GetSpriteAtlasList();

            foreach (var atlas in atlasList)
            {
                if (atlas.name == itemName + atlasPrefix)
                {
                    return atlas.GetSprite(spriteName);
                }
            }
            
            return null;
        }
    }
}

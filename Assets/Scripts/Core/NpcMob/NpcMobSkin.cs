using UnityEngine;
using Zenject;

namespace Playstel
{
    public class NpcMobSkin : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMesh;

        NpcMobStat mobStat;
        UnitVFX unitVFX;
        UnitSFX unitSFX;

        [Inject]
        private CacheMesh _cacheMesh;

        public enum Skin
        {
            Container,
            Material
        }

        public void Awake()
        {
            mobStat = GetComponent<NpcMobStat>();
            unitSFX = GetComponent<UnitSFX>();
            unitVFX = GetComponent<UnitVFX>();
        }

        public void Start()
        {
            ActiveShadow(UnityEngine.Rendering.ShadowCastingMode.On);
        }

        public void ActiveShadow(UnityEngine.Rendering.ShadowCastingMode state)
        {
            skinnedMesh.shadowCastingMode = state;
        }

        string recentMeshContainer;
        public void SetSkinComponents(ItemInfo info)
        {
            var customData = info.GetUnsafeCustomData();

            var meshContainer = DataHandler.GetUnsafeValue(customData, Skin.Container.ToString());

            skinnedMesh.sharedMesh = _cacheMesh.Get(meshContainer);

            unitSFX.SetUnitSFX(info);
            unitVFX.SetUnitVFX(info);
            
            SetSkinStat(info.itemName);
        }

        public void SetSkinStat(string meshContainer)
        {
            if (recentMeshContainer == meshContainer) return;
            recentMeshContainer = meshContainer;
            mobStat.SetStat(meshContainer);
        }
    }
}

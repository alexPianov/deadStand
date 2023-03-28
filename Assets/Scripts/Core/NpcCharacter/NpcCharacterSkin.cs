using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class NpcCharacterSkin : MonoBehaviour
    {
        public ObscuredString currentMeshName;
        private Item _rigItem;
        private SkinnedMeshRenderer _skinnedMesh;
        private Unit _unit;

        public enum Skin
        {
            Mesh, DefaultMaterial, SeasonMaterials, IsMale
        }

        private void Awake()
        {
            _skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
            _unit = GetComponent<Unit>();

            ActiveShadow(PhotonNetwork.InRoom
                ? UnityEngine.Rendering.ShadowCastingMode.On
                : UnityEngine.Rendering.ShadowCastingMode.Off);
        }

        public async UniTask SetRigComponents(Item item)
        {
            var customData = item.info.GetUnsafeCustomData();

            currentMeshName = GetSkinData(customData, Skin.DefaultMaterial);

            await SetSkinMaterial(currentMeshName);
            SetMesh(customData);

            if (!_unit.Renderer.renderIsActive)
            {
                Active(false);
            }
        }

        public void ActiveShadow(UnityEngine.Rendering.ShadowCastingMode state)
        {
            _skinnedMesh.shadowCastingMode = state;
        }
        
        #region Material

        private async UniTask SetSkinMaterial(string materialName)
        {
            _skinnedMesh.material = await AddressablesHandler.Load<Material>(materialName);
        }
        
        private string GetSkinData(Dictionary<string, string> customData, Skin skinData)
        {
            return DataHandler.GetUnsafeValue(customData, skinData.ToString());
        }

        #endregion

        #region Mesh

        [Inject]
        private CacheMesh _cacheMesh;

        private void SetMesh(Dictionary<string, string> customData)
        {
            currentMeshName = GetSkinData(customData, Skin.Mesh);
            _skinnedMesh.sharedMesh = _cacheMesh.Get(currentMeshName);
        }
        
        public void Active(bool state)
        {
            _skinnedMesh.enabled = state;
        }
        
        #endregion

    }
}

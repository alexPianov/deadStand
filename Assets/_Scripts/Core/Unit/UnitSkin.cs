using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitSkin : MonoBehaviour
    {
        [HideInInspector] public ObscuredString currentRigName;
        [HideInInspector] public ObscuredString currentMeshName;
        [HideInInspector] public ObscuredString currentMaterialName;
        [HideInInspector] public ObscuredString currentMaterialType;
        [HideInInspector] public ObscuredBool isMale;

        private Item _rigItem;
        private SkinnedMeshRenderer _skinnedMesh;
        private Unit _unit;

        [Inject] private CacheTitleData _cacheTitleData;

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

        public async UniTask SetRigComponents(Item item, string skinMaterial)
        {
            if(item == _rigItem) return;

            _rigItem = item;
            currentRigName = item.info.itemName;

            var customData = item.info.GetUnsafeCustomData();

            SetSex(customData);
            InitializeSkinMaterials(customData);

            if (string.IsNullOrEmpty(skinMaterial))
            {
                Debug.Log("SkinMaterial is null");
                skinMaterial = GetSkinData(customData, Skin.DefaultMaterial);
            }

            await SelectSkinFromMaterials(skinMaterial);

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

        private void SetSex(Dictionary<string, string> customData)
        {
            var result = GetSkinData(customData, Skin.IsMale);
            if (string.IsNullOrEmpty(result)) return;
            isMale = bool.Parse(result);
        }

        #region Mesh

        [Inject]
        private CacheMesh _cacheMesh;

        private void SetMesh(Dictionary<string, string> customData)
        {
            currentMeshName = GetSkinData(customData, Skin.Mesh);
            _skinnedMesh.sharedMesh = _cacheMesh.Get(currentMeshName);
        }
        
        #endregion

        #region Material

        private List<string> materialTypes = new List<string>() { "Light", "Dark", "Yellow" };
        public Dictionary<string, List<string>> skinMaterialNames = new();
        
        private async UniTask InitializeSkinMaterials(Dictionary<string, string> customData)
        {
            var seasonMaterials = GetSkinData(customData, Skin.SeasonMaterials);
            if (seasonMaterials == null) return;

            var materialsData = _cacheTitleData.GetTitleData(seasonMaterials);
            if(materialsData == null) return;

            var rawSkinData = DataHandler.Deserialize(materialsData);
            if(rawSkinData == null) return;

            foreach (var skinType in materialTypes)
            {
                rawSkinData.TryGetValue(skinType, out string value);
                
                var materialNames = DataHandler.SplitString(value);
                
                if (skinMaterialNames.ContainsKey(skinType))
                {
                    skinMaterialNames.Remove(skinType);
                }
                
                skinMaterialNames.Add(skinType, materialNames);
            }
        }

        public async UniTask SelectSkinFromMaterials(string materialName)
        {
            if (skinMaterialNames == null)
            {
                Debug.Log("Failed to find skin material names"); return;
            }
            
            foreach (var materialType in materialTypes)
            {
                skinMaterialNames.TryGetValue(materialType, out var materialNames);

                if(materialNames == null) continue;
                
                for (var i = 0; i < materialNames.Count; i++)
                {
                    if (materialNames[i] == materialName)
                    {
                        currentMaterialName = materialName;
                        currentMaterialType = materialType;
                        
                        await SetSkinMaterial(materialName);
                    }
                }
            }
        }

        private async UniTask SetSkinMaterial(string materialName)
        {
            _skinnedMesh.material = await AddressablesHandler.Load<Material>(materialName);
        }

        public void Active(bool state)
        {
            _skinnedMesh.enabled = state;
        }

        public void SetLayer(int layer)
        {
            _skinnedMesh.gameObject.layer = layer;
        }

        #endregion
        
        private string GetSkinData(Dictionary<string, string> customData, Skin skinData)
        {
            return DataHandler.GetUnsafeValue(customData, skinData.ToString());
        }
    }
}

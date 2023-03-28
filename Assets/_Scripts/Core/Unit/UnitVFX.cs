using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class UnitVFX : MonoBehaviourPun
    {
        List<string> visuals = new List<string> { "V_Spw", "V_Dmg", "V_Dth", "V_Dec", "V_Atk",
        "V_Brn", "V_Shk", "V_Psn", "V_Blw", "V_Exp", "V_Hea", "V_Spd" };

        public Transform effectsContainer;
        public Transform externalEffectsContainer;

        public enum Visual
        {
            Spw, Dmg, Dth, Dec, Atk, Brn, Shk, Psn, Blw, Exp, Hea, Spd
        }

        public void Awake()
        {
            if (PhotonNetwork.InRoom && photonView.Owner != null)
            {
                externalEffectsContainer = new GameObject().transform;
                externalEffectsContainer.SetParent(null);
                externalEffectsContainer.name = photonView.Owner.NickName + " Effects";
            }
        }

        Dictionary<string, List<string>> visualCollection = new ();

        [Inject]
        private CacheItemInfo _cacheItemInfo;
        private const string _setupKey = "Setup";
        private string _recentVfxSetup;
        public void SetUnitVFX(ItemInfo itemInfo)
        {
            var setupName = DataHandler.GetUnsafeValue
                (itemInfo.GetUnsafeCustomData(), _setupKey);
            
            if(_recentVfxSetup == setupName) return;
            _recentVfxSetup = setupName;
            
            var customData = _cacheItemInfo
                .GetItemInfo(setupName, ItemInfo.Catalog.Setup, ItemInfo.Class.Unit)
                .GetTypedData(ItemInfo.DataType.FX);

            if (customData == null) { Debug.Log(setupName + " doesn't contains FX key"); return; };

            foreach (var visual in visuals)
            {
                if (visualCollection.ContainsKey(visual)) continue;
                AddVisualsToCollection(customData, visual);
            }
        }

        private void AddVisualsToCollection(Dictionary<string, string> customData, string visualListName)
        {
            var visualString = GetStatString(customData, visualListName);

            if (string.IsNullOrEmpty(visualString))
            {
                return;
            }

            var visualList = DataHandler.SplitString(visualString);

            visualCollection.Add(visualListName, visualList);
        }

        private string GetStatString(Dictionary<string, string> customData, string statName)
        {
            return DataHandler.GetUnsafeValue(customData, statName);
        }

        public List<GameObject> _effects = new ();
        public async UniTask Create(Visual visual, bool toEffectsContainer = false, Transform parent = null)
        {
            if (toEffectsContainer) parent = externalEffectsContainer;
            
            var effectName = GetRandomVisualName(visual);
            
            var effect = await AddressablesHandler.Get(effectName, parent);

            if (!effect) { Debug.LogWarning("Failed to create Unit VFX " + effectName); return; }

            effect.transform.SetParent(parent);

            RemoveNullEffects();
            
            _effects.Add(effect);
        }

        private void RemoveNullEffects()
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                if (_effects[i] == null) _effects.Remove(_effects[i]);
            }
        }

        private string GetRandomVisualName(Visual visual)
        {
            visualCollection.TryGetValue("V_" + visual, out List<string> visuals);
            if (visuals == null) return null;
            if (visuals.Count == 0) return null;
            var num = Random.Range(0, visuals.Count - 1);
            return visuals[num];
        }

        public void StopAllEffects()
        {
            RemoveNullEffects();
            
            for (int i = 0; i < _effects.Count; i++)
            {
                if(!_effects[i]) continue;
                
                _effects[i].transform.SetParent(null);
                
                if (_effects[i].GetComponent<ParticleSystem>())
                {
                    _effects[i].GetComponent<ParticleSystem>().Stop();
                }
            }
        }

        private void LateUpdate()
        {
            EffectsNullRot();
        }

        Quaternion nullRotation = new Quaternion(0, 0, 0, 0);
        private void EffectsNullRot()
        {
            if(_effects == null) return;
            if(_effects.Count == 0) return;
            
            if (effectsContainer && externalEffectsContainer)
            {
                externalEffectsContainer.position = effectsContainer.position;
                externalEffectsContainer.rotation = nullRotation;
            }
        }

        public void OnDestroy()
        {
            if(externalEffectsContainer) Destroy(externalEffectsContainer.gameObject);
        }

        public async void CreateSplash()
        {
            var grabFx = await AddressablesHandler.Get(KeyStore.VFX_GRAB);
            grabFx.transform.rotation = Quaternion.identity;
            grabFx.transform.position = transform.position;
        }
    }
}

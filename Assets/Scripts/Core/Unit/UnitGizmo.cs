using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitGizmo : MonoBehaviourPun
    {
        [Header("Refs")]
        public MeshRenderer unitIcon;
        public MeshRenderer arrow;
        public MeshRenderer backgroundGlow;
        
        [Inject] private CacheGizmos _cacheGizmos;

        public void SetPlayerGizmo()
        {
            if (TryGetComponent(out UnitFraction fraction))
            {
                Arrow(photonView.IsMine);
                PlayerIcon(fraction);
            }
        }

        private void PlayerIcon(UnitFraction fraction)
        {
            unitIcon.material = LoadMaterial("Round_" + fraction.currentFraction);
            backgroundGlow.material = LoadMaterial("Glow_" + fraction.currentFraction);
        }
        
        private void Arrow(bool state)
        {
            arrow.enabled = state;
            
            if (state)
            {
                arrow.material = LoadMaterial("Arrow");
            }
        }

        private const string glowNpc = "Glow_Npc";
        public void SetNpcGizmo(string npcName)
        {
            if(arrow) arrow.enabled = false;
            unitIcon.material = LoadMaterial(npcName);
            backgroundGlow.material = LoadMaterial(glowNpc);
        }

        public void ActiveNpcGizmo(bool state)
        {
            unitIcon.enabled = state;
            backgroundGlow.enabled = state;
        }


        private const string prefix = "Gizmos_";
        private Material LoadMaterial(string materialName)
        {
            if (_cacheGizmos == null)
            {
                return null;
            }
            
            return _cacheGizmos.Get(prefix + materialName);
        }
    }
}

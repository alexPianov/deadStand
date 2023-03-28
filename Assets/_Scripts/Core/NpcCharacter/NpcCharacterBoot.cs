using Cysharp.Threading.Tasks;
using EventBusSystem;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class NpcCharacterBoot : MonoBehaviour
    {
        [Header("Setup")]
        public NpcCharacterProfile characterProfile;
        public GameObject characterPrefab;
        
        [Header("Custom Animation")]
        public bool isKinematic;
        public bool idleAnimation;
        public bool activeGizmos = true;
        public string animationName;

        [Header("Gizmos")]
        public Mesh characterGizmoMesh;

        [Inject]
        private CacheItemInfo _cacheItemInfo;

        private async void Start()
        {
            var characterInstance = CreateNpcInstance();
            
            var characterInfo = GetNpcInfo(_cacheItemInfo);

            await InitializeCharacter(characterInstance, characterInfo);

            SetRigidbodyConstraints(characterInstance);
            
            SyncGizmosAngle(characterInstance);
        }

        private const int _gizmosAngleOffset = 15;
        private static void SyncGizmosAngle(GameObject characterInstance)
        {
            var completeAngleOffset = characterInstance.transform.rotation.y + _gizmosAngleOffset;
            
            EventBus.RaiseEvent<IGizmosRotateHandler>(h => h.HandleCameraRotationChange
                (-completeAngleOffset));
        }

        private void SetRigidbodyConstraints(GameObject characterInstance)
        {
            var rigidbody = characterInstance.GetComponent<Rigidbody>();

            if (!rigidbody) return;

            rigidbody.isKinematic = isKinematic;

            if (isKinematic)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private GameObject CreateNpcInstance()
        {
            return Instantiate
                (characterPrefab, transform.position, transform.rotation, transform);
        }

        private ItemInfo GetNpcInfo(CacheItemInfo _cacheItemInfo)
        {
            var infoList = _cacheItemInfo.GetItemInfoList
                (ItemInfo.Catalog.Character, ItemInfo.Class.Scheme, ItemInfo.Subclass.NPC);

            var characterInfo = infoList
                .Find(item => item.itemName == characterProfile.currentCharacter.ToString());
            
            if (!characterInfo)
            {
                Debug.LogError("Failed to find Character Info " + characterProfile.currentCharacter);
            }
            
            return characterInfo;
        }

        private const string _idlePrefix = "Idle_";
        private async UniTask InitializeCharacter(GameObject characterInstance, ItemInfo characterInfo)
        {
            if (!characterInstance.GetComponent<NpcCharacter>())
            {
                Debug.LogError("Failed to find Npc Character in Character Instance"); return;
            }
            
            await characterInstance.GetComponent<NpcCharacter>()
                .Initialize(characterInfo, characterProfile, activeGizmos);

            if (idleAnimation)
            {
                var animator = characterInstance.GetComponent<Animator>();

                if (!string.IsNullOrEmpty(animationName))
                {
                    animator.Play(animationName);
                }
                else
                {
                    animator.Play(_idlePrefix + characterInfo.itemName);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawMesh
                (characterGizmoMesh, 0, transform.position, transform.rotation, new Vector3(1,1,1));
        }
    }
}

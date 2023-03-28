using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Playstel
{
    public class ItemGrenade : MonoBehaviour
    {
        private Unit _unit;
        private Item _item;
        private ItemImpact _impact;
        private ItemStat _itemStat;

        [Header("Sound")]
        AudioSource source;

        [Header("Throw")]
        ParticleSystem trail;

        [Header("Mode")]
        ObscuredInt releaseTimer = 1;
        ObscuredBool activateOnCollision = true;

        [Header("Tags")]
        List<ObscuredString> targetTags = new List<ObscuredString>() { "Unit", "Mob" };

        [Header("Physics")]
        BoxCollider boxCollider;
        Rigidbody body;
        SphereCollider expandSphere;
        
        //[Inject] private CacheImpacts _cacheImpacts;

        ItemSFX itemSFX;

        #region Init Object

        public void Awake()
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            expandSphere = gameObject.AddComponent<SphereCollider>();
            source = gameObject.AddComponent<AudioSource>();
            trail = GetComponentInChildren<ParticleSystem>();
            body = gameObject.AddComponent<Rigidbody>();
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
            itemSFX = GetComponent<ItemSFX>();

            KinematicMode(true);
            expandSphere.enabled = false;
            expandSphere.isTrigger = true;
        }

        public void SetComponents(Item item, Unit unit)
        {
            _unit = unit;
            _item = item;
            _itemStat = GetComponent<ItemStat>();
            //_impact = _cacheImpacts.GetImpact(_itemStat.impactName);
        }

        #endregion

        public void ThrowSwing()
        {
            _unit.GrenadeThrow.ThrowCamDelay();
            _unit.GrenadeThrow.MoveThrowPending();

            _unit.Animation.ItemAnimation(UnitAnimation.Actions.Throw, _item.info);
            _unit.Await.Await(true);
        }

        float startCollisionTime = 0.3f;
        public void Throw(float throwPower, Vector3 pos, Quaternion rot)
        {
            _unit.Await.Await(false);
            //_unitResources.SubtractGrenadeAmmo();

            _unit.GrenadeThrow.GrenadeIsThrowed = true;

            UpdateGrenadeTransform(pos, rot);
            KinematicMode(false);

            if (releaseTimer > 0)
            {
                Invoke(nameof(CreateField), releaseTimer);
            }

            if (activateOnCollision)
            {
                Invoke(nameof(Collisions), startCollisionTime);
            }

            if (trail) trail.Play();

            SetForce(throwPower);
        }

        public void UpdateGrenadeTransform(Vector3 pos, Quaternion rot)
        {
            transform.SetParent(null);
            transform.rotation = rot;
            transform.position = pos;
        }

        public void SetForce(float throwPower)
        {
            body.AddForce(transform.forward * throwPower, ForceMode.VelocityChange);
        }

        private void KinematicMode(bool state)
        {
            body.isKinematic = state;
            boxCollider.isTrigger = state;
        }

        bool collisions;
        private void Collisions()
        {
            collisions = true;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collisions)
            {
                Debug.Log("OnCollisionEnter");
                CreateField();
            }
        }

        #region Field

        public void CreateField()
        {
            collisions = false;
            CancelInvoke();
            KinematicMode(true);
            ReleaseSetup();
            CreateImpactEffects();

            if (_impact.fieldExpandSpeed > 0)
            {
                ExpandingSphere();
            }
            else
            {
                StaticSphere();
            }

            Invoke(nameof(RemoveObject), _impact.fieldLifetime * 1.5f);
        }

        private void RemoveObject()
        {
            calculateRate = false;
            Destroy(gameObject);
        }

        Quaternion startRot = new Quaternion(0, 0, 0, 1);
        private void ReleaseSetup()
        {
            expandSphere.enabled = true;
            transform.localRotation = startRot;
            GetComponent<MeshRenderer>().enabled = false;
        }

        #endregion

        #region Static Sphere

        private void StaticSphere()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _impact.fieldRange);

            Debug.Log("Colliders Count: " + colliders.Length);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (targetTags.Contains(colliders[i].tag))
                    SendToCollider(colliders[i]);
            }
        }

        #endregion

        #region Expanding Sphere

        [Header("Modes")]
        bool expand;
        bool calculateRate;
        private void ExpandingSphere()
        {
            expandSphere.enabled = true;
            expand = true;
            calculateRate = true;
        }

        float activeTime = 0;
        private void FixedUpdate()
        {
            if (expand)
            {
                expandSphere.radius += Time.deltaTime * _impact.fieldExpandSpeed;

                if (expandSphere.radius >= _impact.fieldRange)
                    expand = false;
            }

            if (calculateRate)
            {
                activeTime += Time.deltaTime;

                if (activeTime >= _impact.fieldLifetime)
                    calculateRate = false;
            }
        }

        float timeInField;
        private void OnTriggerStay(Collider other)
        {
            if (!_impact) return;

            if (!calculateRate) return;

            if (targetTags.Contains(other.tag))
            {
                timeInField += Time.deltaTime;

                if (timeInField > _impact.fieldRate)
                {
                    timeInField = 0;
                    SendToCollider(other);
                }
            }
        }

        #endregion

        #region Send Impact

        private void SendToCollider(Collider collider)
        {
            // if (collider.TryGetComponent(out UnitCallbackBuffer callbackBuffer))
            // {
            //     callbackBuffer.ReceiveImpact(_itemStat.impactName, _unit.GetViewId(), true);
            // }
        }

        #endregion

        #region Effects

        private void CreateImpactEffects()
        {
            CreateEffect(_impact.effectOrigin, false);
            //itemSFX.CreateSound(_impact.soundOrigin);

            CreateEffect(_impact.effectExpand, true);
            //itemSFX.CreateSound(_impact.soundExpand);
        }

        private async UniTask CreateEffect(string effectName, bool customLifetime)
        {
            if (string.IsNullOrEmpty(effectName)) return;

            Debug.Log("CreateEffect: " + effectName);

            GameObject effect = await AddressablesHandler.Get(effectName, transform);

            effect.transform.rotation = transform.rotation;
            effect.transform.position = transform.position;

            effect.layer = 9;

            if (customLifetime) EditParticleDuration(effect);
        }

        private void EditParticleDuration(GameObject effect)
        {
            if (!effect.GetComponent<ParticleSystem>())
            {
                Debug.LogError("Failed to find ParticleSystem");
                return;
            }

            effect.SetActive(false);
            var particleSettings = effect.GetComponent<ParticleSystem>().main;
            particleSettings.duration = _impact.fieldLifetime + 2.5f;
            effect.SetActive(true);
        }

        #endregion

    }
}

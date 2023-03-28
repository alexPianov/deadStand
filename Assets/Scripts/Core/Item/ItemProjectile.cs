using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Playstel
{
    [RequireComponent(typeof(Rigidbody))]
    public class ItemProjectile : MonoBehaviourPun
    {
        private ItemFirearm _firearm;

        public ObscuredInt _bulletHostFraction;
        public ObscuredInt _bulletHostID;
        private ObscuredInt _criticalRank = 2;
        private ObscuredInt _damageAmount;
        private BoxCollider _boxCollider;
        private MeshRenderer[] _renderers;
        private MeshRenderer _itemRenderer;
        private Rigidbody _body;
        private AudioSource _audioSource;

        private bool _disableCollisionEffect;

        public void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _itemRenderer = GetComponent<MeshRenderer>();
            _audioSource = GetComponent<AudioSource>();

            Physics(false);
        }

        private void Renderer(bool state)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = state;
            }

            _itemRenderer.enabled = state;
        }
        
        public void StartBullet(ItemFirearm firearm)
        {
            _firearm = firearm;

            Physics(true);
            CollisionEffect();
            SetBulletHostData();
            SetDamageAmount();
            SetImpulse();
            SetLifeTime();
        }

        private void CollisionEffect()
        {
            if (_firearm.Ammo.GetAmmoInfo().itemName == "Fuel")
            {
                _disableCollisionEffect = true;
            }
        }

        private void Physics(bool state)
        {
            _boxCollider.isTrigger = !state;
            _body.isKinematic = !state;
            
            Renderer(state);
        }

        private void SetBulletHostData()
        {
            if(!_firearm.Unit) return;

            _bulletHostID = _firearm.Unit.GetViewId();
            _bulletHostFraction = (int)_firearm.Unit.Fraction.currentFraction;
        }

        private void SetDamageAmount()
        {
            _damageAmount = _firearm.Stat.damage;
        }

        public void SetImpulse()
        {
            _body.velocity = transform.forward * _firearm.Ammo.speed;
        }

        private int bulletLifeTime = 1500;
        private async void SetLifeTime()
        {
            await UniTask.Delay(bulletLifeTime);
            
            Physics(false);
            ReturnBullet();
        }

        private const int areaLayerNumber = 14;
        private async void OnCollisionEnter(Collision collision)
        {
            Physics(false);

            if (collision.gameObject.layer == areaLayerNumber)
            {
                CollisionEffect(collision, KeyStore.VFX_DUST);
                await CollisionSound(CacheSoundClips.HitType.Solid);
                ReturnBullet();
                return;
            }
            
            if (collision.gameObject.TryGetComponent(out Unit collisionUnit))
            {
                if (collisionUnit.Fraction && 
                    (int)collisionUnit.Fraction.currentFraction != _bulletHostFraction)
                {
                    SendDamage(collisionUnit, collision);
                    CheckImpact(collisionUnit);
                }
                
                CollisionEffect(collision, KeyStore.VFX_BLOOD);
                await CollisionSound(CacheSoundClips.HitType.Flesh);
            }
            else
            {
                CollisionEffect(collision, KeyStore.VFX_DUST);
                await CollisionSound(CacheSoundClips.HitType.Solid);
            }
            
            ReturnBullet();
        }

        private void SendDamage(Unit collisionUnit, Collision collision)
        {
            collisionUnit.Damage.SetDamage
                (_damageAmount, _bulletHostID, collision.GetContact(0).point);
        }

        private void CheckImpact(Unit unit)
        {
            var impact = _firearm.Stat.impactName;
            if(string.IsNullOrEmpty(impact)) return;
            unit.Callback.ReceiveImpact(impact, _bulletHostID);
        }

        private async UniTask CollisionEffect(Collision collision, string effectName)
        {
            if(_disableCollisionEffect) return;
            
            var contact = collision.GetContact(0);

            GameObject fx = await AddressablesHandler.Get(effectName, 
                collision.gameObject.transform);
            
            fx.transform.rotation = Quaternion.LookRotation(contact.normal);
            fx.transform.position = contact.point;
        }

        private async UniTask CollisionSound(CacheSoundClips.HitType hitType)
        {
            if(_disableCollisionEffect) return;
            if(!_audioSource) return;

            var collisionClip = _firearm.Effects.CacheSoundClips.GetRandomHitSound(hitType);
            _audioSource.PlayOneShot(collisionClip);
            
            await UniTask.WaitUntil(() => !_audioSource.isPlaying);
        }

        private void ReturnBullet()
        {
            _firearm.Ammo.Enqueue(gameObject);
        }
    }
}

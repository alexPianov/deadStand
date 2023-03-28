using System;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class LootFragToken : MonoBehaviourPun
    {
        public UnitFraction.Fraction _tokenFraction;
        public ParticleSystem ParticleSystem;
        public Color blueColor = new Color(0,77,255,255);

        private int lifeTime = 30;
        private float time;
        private void Update()
        {
            if(isUsed) return;
            
            time += Time.deltaTime;

            if (time > lifeTime)
            {
                DisableCollider();
                CreateSplash();
            }
        }

        public void SetFraction(UnitFraction.Fraction fraction)
        {
            _tokenFraction = fraction;

            name = "Player Token (" + _tokenFraction + ")";
            
            if (_tokenFraction == UnitFraction.Fraction.Blue)
            {
                TokenColor(blueColor);
            }
                
            if (_tokenFraction == UnitFraction.Fraction.Red)
            {
                TokenColor(Color.red);
            }
        }

        private void TokenColor(Color color)
        {
            var settings = ParticleSystem.main;
            settings.startColor = new ParticleSystem.MinMaxGradient(color);
        }

        
        public void OnTriggerEnter(Collider other)
        {
            if (isUsed) return;
            
            if (other.TryGetComponent(out Unit otherUnit))
            {
                if (otherUnit.HostOperator == null) return;
                if (otherUnit.Fraction == null) return;
                if (otherUnit.Fraction.currentFraction == _tokenFraction) return;

                DisableCollider();
                AddToken(otherUnit);
                CreateSplash();
            }
        }

        private bool isUsed;
        private void DisableCollider()
        {
            GetComponent<CapsuleCollider>().enabled = false;
        }
        
        private void AddToken(Unit unit)
        {
            Debug.Log("AddToken");
            
            if(!unit.photonView.IsMine) return;

            unit.Tokens.TakeToken(transform);
        }

        private async void CreateSplash()
        {
            Debug.Log("Create Splash");

            isUsed = true;
            
            var grabFx = await AddressablesHandler.Get(KeyStore.VFX_GRAB);
            grabFx.transform.rotation = Quaternion.identity;
            grabFx.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}


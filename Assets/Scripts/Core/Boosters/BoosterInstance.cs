using UnityEngine;
using static Playstel.EnumStore;

namespace Playstel
{
    public class BoosterInstance : MonoBehaviour
    {
        public ItemBooster itemBooster;
        
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

        public void OnTriggerEnter(Collider other)
        {
            if (isUsed) return;
            
            if (other.TryGetComponent(out Unit otherUnit))
            {
                if (otherUnit.HostOperator == null) return;

                DisableCollider();
                AddBooster(otherUnit);
                CreateSplash();
            }
        }

        private bool isUsed;
        private void DisableCollider()
        {
            GetComponent<CapsuleCollider>().enabled = false;
        }
        
        private void AddBooster(Unit unit)
        {
            Debug.Log("AddToken");
            
            if(!unit.photonView.IsMine) return;

            unit.Booster.TakeBooster(itemBooster, transform);
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

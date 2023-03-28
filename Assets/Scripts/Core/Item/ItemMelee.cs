using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ItemMelee : MonoBehaviour
    {
        private Unit _unit;
        private Item _item;

        [HideInInspector] public ItemStat itemStat;
        [HideInInspector] public ItemSFX itemSFX;

        BoxCollider boxCollider;

        public void SetComponents(Item item, Unit unit)
        {
            _item = item;
            _unit = unit;

            itemStat = GetComponent<ItemStat>();
            
            boxCollider = gameObject.AddComponent<BoxCollider>();
            itemSFX = gameObject.GetComponent<ItemSFX>();
        }

        #region Use

        public void Use()
        {
            Debug.Log("Use " + _item.info.itemName);

            _unit.Animation.ItemAnimation(UnitAnimation.Actions.Melee, _item.info);
            _unit.Animation.AwaitAnimation();

            //itemSFX.PlaySound(ItemSFX.Sounds.Swg);
        }

        public void Damage(bool state)
        {
            if(boxCollider) boxCollider.isTrigger = state;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Unit unit))
            {
                var collisionId = unit.photonView.ViewID;
                
                if (collisionId == _unit.GetViewId()) return;
                
                // callbackBuffer.ReceiveDamage(itemStat.damage, 
                //     collision.contacts[0].point, _unit.GetViewId());

                //itemSFX.PlaySound(ItemSFX.Sounds.Hit);
            }
            else
            {
                //itemSFX.PlaySound(ItemSFX.Sounds.Sld);
                CollisionEffect(collision);
            }
        }

        private async void CollisionEffect(Collision collision)
        {
            var contact = collision.GetContact(0);

            GameObject fx = await AddressablesHandler.Get(KeyStore.VFX_DUST, collision.gameObject.transform);
            
            fx.transform.rotation = Quaternion.LookRotation(contact.normal);
            fx.transform.position = contact.point;
        }


        #endregion
    }
}

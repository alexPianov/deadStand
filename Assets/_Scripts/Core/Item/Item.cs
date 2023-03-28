using UnityEngine;

namespace Playstel
{
    public class Item : ScriptableObject
    {
        public ItemInfo info;
        public GameObject instance;

        public void SetInfo(ItemInfo itemInfo)
        {
            if(itemInfo == null)
            {
                Debug.LogError("Failed to find itemInfo");
                return;
            }

            info = itemInfo;
            name = itemInfo.itemName;
        }

        public void SetItemInstance(GameObject itemObject)
        {
            instance = itemObject;
        }

        public void DropInstance()
        {
            if(!instance) { Debug.LogError("Failed to find instance for " + info.itemName); return; }

            if (!instance.GetComponent<BoxCollider>())
            {
                instance.AddComponent<BoxCollider>();
            }
            else
            {
                instance.GetComponent<BoxCollider>().isTrigger = false;
            }

            if (!instance.GetComponent<Rigidbody>())
            {
                instance.AddComponent<Rigidbody>().collisionDetectionMode
                    = CollisionDetectionMode.Continuous;
            }

            instance.transform.SetParent(null);
        }
        
        public void DestroyInstance()
        {
            if(!instance) { return; }

            Destroy(instance);
        }

        public void DestroyItem()
        {
            info = null;
            Destroy(this);
        }
    }
}

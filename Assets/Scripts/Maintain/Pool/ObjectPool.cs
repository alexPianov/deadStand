using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class ObjectPool : MonoBehaviour
    {
        #region Pool

        public List<Pool> pools = new List<Pool>();

        [System.Serializable]
        public class Pool
        {
            [Header("Info")]
            public string objectName;
            public string objectMaterial;
            public int poolSize;
            public Transform poolParent;

            [Header("Created Objects")]
            public Queue<GameObject> queue = new Queue<GameObject>();
            public List<GameObject> objects = new List<GameObject>();

            public void RemovePool()
            {
                queue.Clear();

                for (int i = 0; i < objects.Count; i++)
                {
                    Destroy(objects[i]);
                }

                objects.Clear();
            }
        }

        #endregion

        #region Create Pool
 
        public async UniTask CreatePool(string itemName, int size, string mat, Transform parent)
        {
            CheckDuplicate(itemName);

            Pool pool = new Pool();
            pool.objectName = itemName;
            pool.objectMaterial = mat;
            pool.poolSize = size;
            pool.poolParent = parent;
            pools.Add(pool);

            await CreatePoolObjects(pool);
        }

        public async UniTask CreatePoolObjects(Pool pool)
        {
            for (int i = 0; i < pool.poolSize; i++)
            {
                await CreateObject(pool);
            }
        }

        private async UniTask CreateObject(Pool pool)
        {
            var obj = await AddressablesHandler.Get(pool.objectName, pool.poolParent);

            pool.queue.Enqueue(obj);

            obj.name = pool.objectName;
            obj.SetActive(false);

            SetMaterialToMeshRenderers(obj, pool.objectMaterial);

            AddEnqueueOnDisable(obj, pool);
        }

        private async void SetMaterialToMeshRenderers(GameObject obj, string matName)
        {
            if (string.IsNullOrEmpty(matName)) return;

            obj.GetComponent<MeshRenderer>().material = 
                await AddressablesHandler.Load<Material>(matName);
        }

        public void AddEnqueueOnDisable(GameObject instance, Pool pool)
        {
            var notify = instance.AddComponent<NotifyOnDisable>();

            notify.instanceRef = instance;
            notify.queue = pool.queue;
        }

        public void Enqueue(GameObject obj)
        {
            obj.transform.SetParent(transform);
            obj.GetComponent<NotifyOnDisable>().queue.Enqueue(obj);
            obj.SetActive(false);
        }

        private void CheckDuplicate(string objectName)
        {
            if(pools == null || pools.Count == 0) return;
            
            var poolsList = pools.FindAll(pool => pool.objectName == objectName);

            if (poolsList.Count > 0)
            {
                foreach (var pool in poolsList)
                {
                    pool.RemovePool();
                    pools.Remove(pool);
                }
            }
        }

        #endregion

        #region Get Object

        public GameObject GetFromPool(string poolName, Transform targetParent = null)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                var pool = pools[i];

                if (poolName == pool.objectName)
                {
                    if (pool.queue.Count == 0)
                    {
                        Debug.LogError("Pool count is null | " + pool.objectName);
                        return null;
                    }

                    var obj = pool.queue.Dequeue();

                    obj.transform.SetParent(targetParent);

                    obj.SetActive(true);

                    return obj;
                }
            }

            return null;
        }

        #endregion

    }
}

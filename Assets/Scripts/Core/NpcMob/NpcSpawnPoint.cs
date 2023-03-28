using CodeStage.AntiCheat.ObscuredTypes;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Playstel
{
    public class NpcSpawnPoint : MonoBehaviourPun
    {
        [Header("Spawn")]
        public NpcSpawn spawn;

        [Header("Setup")]
        public ObscuredBool spawning;

        #region Pool

        Dictionary<string, Queue<GameObject>> poolDictionary =
            new Dictionary<string, Queue<GameObject>>();

        private void Start()
        {
            photonView.RPC(nameof(RPC_SetPools), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RPC_SetPools()
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < spawn.size; i++)
            {
                GameObject newMob = PhotonNetwork.Instantiate(Path.Combine
                 ("Photon", spawn.mobName), transform.position, Quaternion.identity);

                newMob.transform.SetParent(transform);
                newMob.SetActive(false);
                objectPool.Enqueue(newMob);
            }

            poolDictionary.Add(spawn.mobName, objectPool);
        }

        private GameObject GetMobFromPool()
        {
            if (!poolDictionary.ContainsKey(spawn.mobName)) return null;

            GameObject mobInstance = poolDictionary[spawn.mobName].Dequeue();

            mobInstance.SetActive(true);

            SpawnPointTransform(mobInstance);

            return mobInstance;
        }

        private void SpawnPointTransform(GameObject mob)
        {
            mob.transform.position = transform.position + Random.insideUnitSphere * spawn.range;
        }

        public void ReturnToPool(GameObject mob)
        {
            Debug.Log("Return to pool " + mob.name);
            poolDictionary[spawn.mobName].Enqueue(mob);
            mob.SetActive(false);
            spawnedMobs.Remove(mob);
        }

        #endregion

        #region Spawner

        [Header("Mobs")]
        public List<GameObject> spawnedMobs;

        float spawnTimer;
        private void FixedUpdate()
        {
            if (!spawning) return;

            if (spawnedMobs.Count < spawn.size)
            {
                spawnTimer += Time.deltaTime;

                if (spawn.rate >= spawnTimer)
                {
                    spawnTimer = 0;
                    Spawn();
                }
            }
        }

        public void Spawn()
        {
            photonView.RPC(nameof(RPC_Spawn), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RPC_Spawn()
        {
            GameObject mob = GetMobFromPool();

            if (spawnedMobs.Contains(mob)) return;

            spawnedMobs.Add(mob);

            mob.transform.position = transform.position;
            mob.GetComponent<NpcMobBuilder>().StartMob(spawn.mobName, this);
        }

        #endregion
    }
}

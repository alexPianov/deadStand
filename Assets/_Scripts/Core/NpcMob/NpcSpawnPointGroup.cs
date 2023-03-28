using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class NpcSpawnPointGroup : MonoBehaviour
    {
        public static NpcSpawnPointGroup pointGroup;

        [Header("Spawns")]
        public List<NpcSpawnPoint> spawns = new List<NpcSpawnPoint>();

        #region Singleton

        public void Awake()
        {
            if (!pointGroup)
            {
                pointGroup = this;
            }
            else
            {
                if (pointGroup != this)
                {
                    Debug.Log("Destroy " + name);
                    Destroy(pointGroup.gameObject);
                    pointGroup = this;
                }
            }
        }

        #endregion


        private void Start()
        {
            spawns = DataHandler.GetChildrens<NpcSpawnPoint>(gameObject);
        }

        public List<NpcSpawnPoint> GetSpawnPoints()
        {
            return spawns;
        }
    }
}

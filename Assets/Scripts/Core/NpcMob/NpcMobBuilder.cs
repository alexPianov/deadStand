using Photon.Pun;
using Zenject;

namespace Playstel
{
    public class NpcMobBuilder : MonoBehaviourPun
    {
        NpcSpawnPoint spawnPoint;

        [Inject] private CacheItemInfo _cacheItemInfo;

        public NpcSpawnPoint GetSpawnPoint()
        {
            return spawnPoint;
        }

        public void StartMob(string mobName, NpcSpawnPoint spawn)
        {
            spawnPoint = spawn;

            var mob = _cacheItemInfo.GetItemInfo(mobName);

            GetComponent<NpcMobSkin>().SetSkinComponents(mob);
            GetComponent<NpcMobController>().Restart();
            //GetComponent<UnitRagdoll>().RagdollMode(false);
            GetComponent<UnitRenderer>().Active(true);
            GetComponent<UnitMove>().CanMove(true);
        }

        public void ReturnMob()
        {
            GetSpawnPoint().ReturnToPool(gameObject);
        }
    }
}

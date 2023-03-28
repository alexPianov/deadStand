using Photon.Pun;
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Playstel
{
    public class NpcMobReset : MonoBehaviourPun
    {
        UnitVFX unitVFX;
        NpcMobStat mobStat;
        NpcMobBuilder mobBuilder;

        public void Awake()
        {
            unitVFX = GetComponent<UnitVFX>();
            mobStat = GetComponent<NpcMobStat>();
            mobBuilder = GetComponent<NpcMobBuilder>();
        }

        public async UniTask DropLoot()
        {
            if (string.IsNullOrEmpty(mobStat.loot)) return;

            if (Random.Range(0, 99) > mobStat.lootChance) return;

            var loot = await AddressablesHandler.Get(mobStat.loot, null);

            if (!loot) return;

            loot.transform.position += transform.position + new Vector3(0, 0.25f, 0);
        }

        public void ReturnMob()
        {
            StartCoroutine(Return());
        }

        IEnumerator Return()
        {
            Debug.Log("Return start | respawnTime: " + mobStat.respawnTime);

            yield return new WaitForSeconds(mobStat.respawnTime);

            unitVFX.Create(UnitVFX.Visual.Dec);
            //unitAudio.PlayRandom(UnitSFX.Sounds.Dec, UnitAudio.Type.Damage);

            mobBuilder.ReturnMob();

            StopAllCoroutines();
        }
    }
}

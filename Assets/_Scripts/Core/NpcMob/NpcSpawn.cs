using CodeStage.AntiCheat.ObscuredTypes;
using PlayFab.ClientModels;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Npc/Spawn")]
    public class NpcSpawn : ScriptableObject
    {
        public ObscuredString mobName;
        public ObscuredInt size;
        public ObscuredInt rate;
        public ObscuredInt range;
    }
}

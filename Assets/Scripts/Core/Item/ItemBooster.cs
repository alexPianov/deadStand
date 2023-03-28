using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using static Playstel.EnumStore;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Item/Booster")]
    public class ItemBooster : ScriptableObject
    {
        public BoosterType boosterType;
        public BoosterName boosterName;
        public GameObject boosterPrefab;
    }
}
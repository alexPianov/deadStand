using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Npc/Profile")]
    public class NpcCharacterProfile: ScriptableObject
    {
        [Header("Profile")]
        public Character currentCharacter;
        public UnitFraction.Fraction characterFraction;

        [Header("Data")]
        public List<string> info = new List<string>();

        public enum Character
        {
            Chief, Dealer, Cyclops, Nun, Iceman, Sheriff, 
            Fanatic, FirstBandit, SecondBandit, ThirdBandit, 
            Officer, CyclopsGirl
        }
        
        public enum Type
        {
            Info
        }

        public List<string> GetProfilePhrases(Type type = Type.Info)
        {
            switch (type)
            {
                case Type.Info:
                    return info;
                default:
                    return null;
            }
        }
    }
}

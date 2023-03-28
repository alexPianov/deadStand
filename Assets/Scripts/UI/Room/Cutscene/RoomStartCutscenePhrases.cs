using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Room/Cutscene")]
    public class RoomStartCutscenePhrases: ScriptableObject
    {
        public List<string> phrases = new ();

        public List<string> GetCutscenePhrases()
        {
            return phrases;
        }

    }
}

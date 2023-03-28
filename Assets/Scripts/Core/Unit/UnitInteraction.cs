using Photon.Pun;

namespace Playstel
{
    public class UnitInteraction : MonoBehaviourPun
    {
        public NpcCharacterDialog DialogNpc;
        public Crate Crate;

        public void SetDialogNPC(NpcCharacterDialog _character)
        {
            DialogNpc = _character;
        }

        public void SetCrate(Crate crate)
        {
            Crate = crate;
        }
    }
}

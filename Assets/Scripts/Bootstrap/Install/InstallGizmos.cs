using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Install/Gizmos")]
    public class InstallGizmos : ScriptableObject
    {
        public List<Material> gizmosList;
        IList<string> gizmosLabel = new List<string> { "Gizmos" };

        public async UniTask Install()
        {
            gizmosList.Clear();
            gizmosList = await AddressablesHandler.LoadLabelAssets<Material>(gizmosLabel);
        }

        public List<Material> GetGizmosList()
        {
            return gizmosList;
        }

    }
}
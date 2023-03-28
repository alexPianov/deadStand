using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Playstel
{
    [CreateAssetMenu(menuName = "Install/Mesh")]
    public class InstallMesh : ScriptableObject
    {
        public List<Mesh> meshList;
        IList<string> meshLabel = new List<string> { "Mesh" };

        public async UniTask Install()
        {
            meshList.Clear();
            meshList = await AddressablesHandler.LoadLabelAssets<Mesh>(meshLabel);
        }

        public List<Mesh> GetMeshList()
        {
            return meshList;
        }
    }
}
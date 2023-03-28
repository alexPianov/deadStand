using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class CacheMesh : MonoBehaviour
    {
        public InstallMesh install;

        public Mesh Get(string meshName = null)
        {
            var list = install.GetMeshList();

            if (meshName == null)
            {
                return list[Random.Range(0, list.Count - 1)];
            }

            return list.Find(info => info.name == meshName);
        }
    }
}
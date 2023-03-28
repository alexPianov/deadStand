using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class CacheGizmos : MonoBehaviour
    {
        public InstallGizmos install;

        public Material Get(string gizmoName = null)
        {
            var list = install.GetGizmosList();

            return list.Find(info => info.name == gizmoName);
        }
    }
}
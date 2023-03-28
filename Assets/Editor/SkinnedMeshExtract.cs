using UnityEngine;
using UnityEditor;

namespace Playstel
{
    public class SkinnedMeshExtract : MonoBehaviour
    {
        public Transform extractTarget;
        public string savePath = "Assets/";
        
        public void Start()
        {
            if(extractTarget == null) SaveMesh(transform);
            else SaveMesh(extractTarget);
        }
        
        public void SaveMesh(Transform meshContainer)
        {
            var skins = meshContainer.GetComponentsInChildren<SkinnedMeshRenderer>();
        
            foreach (var skin in skins)
            {
                SaveMesh(skin, true);
            }
        }
        
        private void SaveMesh(SkinnedMeshRenderer renderer, bool instance)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh Asset", savePath, renderer.sharedMesh.name, "asset");
            if (string.IsNullOrEmpty(path))
                return;
            path = FileUtil.GetProjectRelativePath(path);
            Mesh mesh = renderer.sharedMesh;
            Mesh targetMesh = instance ? Object.Instantiate(mesh) as Mesh : mesh;
            AssetDatabase.CreateAsset(targetMesh, path);
            AssetDatabase.SaveAssets();
        }
    }
}
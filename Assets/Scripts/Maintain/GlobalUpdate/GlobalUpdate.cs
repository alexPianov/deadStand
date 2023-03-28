using UnityEngine;

public class GlobalUpdate : MonoBehaviour
{
    private void Update()
    {
        for (int i = 0; i < MonoCache.allUpdate.Count; i++) MonoCache.allUpdate[i].Tick();
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < MonoCache.allFixedUpdate.Count; i++) MonoCache.allFixedUpdate[i].FixedTick();
    }
    private void LateUpdate()
    {
        for (int i = 0; i < MonoCache.allLateUpdate.Count; i++) MonoCache.allLateUpdate[i].LateTick();
    }
}
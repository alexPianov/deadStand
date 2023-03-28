using System.Collections.Generic;
using Playstel;
using UnityEngine;

public class FractionSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public List<Transform> fractionsSpawnpoints = new ();

    public static FractionSpawner Spawner;
    private void Awake()
    {
        Spawner = this;
    }

    public Vector3 GetSpawn(UnitFraction.Fraction fraction)
    {
        return fractionsSpawnpoints[(int)fraction].position + SpawnSpread();
    }

    private Vector3 SpawnSpread()
    {
        return new Vector3(SpreadX(), 0, SpreadZ());
    }

    private const float spreadRange = 4f;
    private float SpreadX()
    {
        return Random.insideUnitSphere.x * spreadRange;
    }

    private float SpreadZ()
    {
        return Random.insideUnitSphere.z * spreadRange;
    }
}

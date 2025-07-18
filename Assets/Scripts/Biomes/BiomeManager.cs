using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    [Header("Biome Settings")]
    public List<BiomeData> biomePresets;

    [Header("Noise Settings")]
    public int worldWidth = 30;
    public int worldDepth = 30;
    public float tileSpacing = 2f;
    public float heightMultiplier = 3f;
    public float temperatureScale = 0.01f;
    public float moistureScale = 0.01f;
    public int seed = 1337;

    [Header("Volume Settings")]
    public GameObject biomeVolumePrefab;
    public Vector3 volumeSize = new Vector3(10f, 10f, 10f);

    private BiomeType[,] biomeMap;

    private void Start()
    {
        GenerateBiomeMap();
        StartCoroutine(GenerateWorldCoroutine());
    }

    public void GenerateBiomeMap()
    {
        biomeMap = new BiomeType[worldWidth, worldDepth];
        System.Random rng = new System.Random(seed);
        float offsetX = rng.Next(0, 100000);
        float offsetZ = rng.Next(0, 100000);

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldDepth; z++)
            {
                float temp = Mathf.PerlinNoise((x + offsetX) * temperatureScale, (z + offsetZ) * temperatureScale);
                float moisture = Mathf.PerlinNoise((x + offsetX + 1000) * moistureScale, (z + offsetZ + 1000) * moistureScale);

                BiomeData closest = GetClosestBiome(temp, moisture);
                biomeMap[x, z] = closest.biomeType;
            }
        }
    }

    private BiomeData GetClosestBiome(float temp, float moisture)
    {
        BiomeData best = null;
        float minDist = float.MaxValue;

        foreach (var biome in biomePresets)
        {
            float dT = biome.temperature - temp;
            float dM = biome.moisture - moisture;
            float dist = dT * dT + dM * dM;

            if (dist < minDist)
            {
                minDist = dist;
                best = biome;
            }
        }

        return best;
    }

    private IEnumerator GenerateWorldCoroutine()
{
    int chunkSize = Mathf.RoundToInt(volumeSize.x); // assuming square chunks

    for (int x = 0; x < worldWidth; x += chunkSize)
    {
        for (int z = 0; z < worldDepth; z += chunkSize)
        {
            // Get average biome in the chunk
            Dictionary<BiomeType, int> biomeCount = new();

            for (int dx = 0; dx < chunkSize; dx++)
            {
                for (int dz = 0; dz < chunkSize; dz++)
                {
                    int tileX = x + dx;
                    int tileZ = z + dz;

                    if (tileX < worldWidth && tileZ < worldDepth)
                    {
                        BiomeType biome = biomeMap[tileX, tileZ];

                        if (!biomeCount.ContainsKey(biome))
                            biomeCount[biome] = 0;

                        biomeCount[biome]++;
                    }
                }
            }

            // Find the dominant biome in this chunk
            BiomeType dominantBiome = BiomeType.Shallow;
            int maxCount = 0;
            foreach (var kvp in biomeCount)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    dominantBiome = kvp.Key;
                }
            }

            // Spawn a BiomeVolume GameObject
                Vector3 centerPosition = new Vector3(
                x * tileSpacing + (volumeSize.x / 2f),
                0,
                z * tileSpacing + (volumeSize.z / 2f)
            );

            GameObject volume = Instantiate(biomeVolumePrefab, centerPosition, Quaternion.identity, this.transform);
            volume.transform.localScale = volumeSize;

                var BoxCollider = volume.GetComponent<BoxCollider>();
                if (BoxCollider != null)
                {
                    BoxCollider.center = Vector3.zero;
                }

            BiomeVolume biomeVolume = volume.GetComponent<BiomeVolume>();
            if (biomeVolume != null)
            {
                biomeVolume.biomeType = dominantBiome;
            }

            yield return null; // optional: slow down generation
        }
    }
}

    public void GenerateWorld()
    {
        System.Random rng = new System.Random(seed + 999);

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldDepth; z++)
            {
                BiomeType biomeType = biomeMap[x, z];
                BiomeData biome = biomePresets.Find(b => b.biomeType == biomeType);

                if (biome != null && biome.terrainPrefabs.Length > 0)
                {
                    GameObject prefab = biome.terrainPrefabs[rng.Next(biome.terrainPrefabs.Length)];

                    Vector3 position = new Vector3(x * tileSpacing, 0, z * tileSpacing);
                    Instantiate(prefab, position, Quaternion.identity, this.transform);
                }
            }
        }
    }
}

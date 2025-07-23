using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    [Header("Biome Settings")]
    public List<BiomeData> biomePresets;

    [Header("Noise Settings")]
    public int worldWidth = 200;
    public int worldDepth = 1000;
    public int verticalLayers = 4;
    public float layerHeight = 20f; // How tall each vertical biome chunk is
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
        StartCoroutine(Generate3DBiomeVolumes());
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
                float temp = Mathf.PerlinNoise((x + offsetX) * temperatureScale,
                                               (z + offsetZ) * temperatureScale);
                float moisture = Mathf.PerlinNoise((x + offsetX + 1000) * moistureScale,
                                                   (z + offsetZ + 1000) * moistureScale);

                BiomeData closest = GetClosestBiome(temp, moisture, 0f);

                biomeMap[x, z] = closest.biomeType;
            }
        }
    }

    private BiomeData GetClosestBiome(float temp, float moisture, float depthY)
    {
        BiomeData best = null;
        float minDist = float.MaxValue;

        foreach (var biome in biomePresets)
        {
            // Skip biomes that don't apply at this depth
            if (depthY > biome.minDepth || depthY < biome.maxDepth)
                continue;

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
    
    private IEnumerator Generate3DBiomeVolumes()
    {
        int tileCountPerChunk = Mathf.RoundToInt(volumeSize.x);    // e.g. 10 tiles per chunk
        float chunkW = tileCountPerChunk * tileSpacing;            // e.g. 10 × 2 = 20 world units
        float chunkD = tileCountPerChunk * tileSpacing;
        float chunkH = volumeSize.y;                               // this should be your layerHeight

         
        for (int y = 0; y < verticalLayers; y++)
        {
            float depthY = -y * chunkH; // Y is in world units

            for (int x = 0; x < worldWidth; x += tileCountPerChunk)
                for (int z = 0; z < worldDepth; z += tileCountPerChunk)
                {
                    float temp = Mathf.PerlinNoise((x + seed) * temperatureScale,
                                                   (z + seed) * temperatureScale);
                    float moisture = Mathf.PerlinNoise((x + seed + 1000) * moistureScale,
                                                   (z + seed + 1000) * moistureScale);

                    BiomeData data = GetClosestBiome(temp, moisture, depthY);
                    if (data == null) continue;

                    // compute the world‐space center of this chunk
                    Vector3 centerPos = new Vector3(
                        x * tileSpacing + chunkW / 2f,
                        depthY - chunkH / 2f,
                        z * tileSpacing + chunkD / 2f
                    );

                    var volume = Instantiate(
                        biomeVolumePrefab,
                        centerPos,
                        Quaternion.identity,
                        transform
                    );

                    volume.transform.localScale = volumeSize;

                    // resize the collider to cover exactly chunkW × chunkH × chunkD
                    var box = volume.GetComponent<BoxCollider>();
                    if (box != null)
                    {
                        box.center = Vector3.zero;
                    }

                    // assign your biome type
                    var v = volume.GetComponent<BiomeVolume>();
                    if (v != null)
                        v.biomeType = data.biomeType;

                    yield return null;
                }
        }
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

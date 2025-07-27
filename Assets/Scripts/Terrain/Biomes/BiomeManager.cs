using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Media;
using UnityEngine;
using UnityEngine.Rendering;

public class BiomeManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject biomeVolumePrefab;
    public GameObject seaFloorPrefab;
    [Header("Biome Settings")]
    public List<BiomeData> biomePresets;
    [SerializeField] private float chunkWorldSize = 500f;
    [SerializeField] private float chunkHeight = 20f;

    [Header("Noise Settings")]
    public int worldWidth = 200;
    public int worldDepth = 1000;
    public int verticalLayers = 4;
    public float layerHeight = 20f; // How tall each vertical biome chunk is
    public float tileSpacing = 2f;
    public float heightMultiplier = 3f;
    public float temperatureScale = 0.01f;
    public float moistureScale = 0.01f;
    public Vector2 chunkOrigin;
    float offsetX = 0;
    float offsetZ = 0;
    public int seed = 1337;

    [Header("Volume Settings")]

    public Vector3 volumeSize = new Vector3(10f, 10f, 10f);

    private System.Random rng;
    private BiomeType[,] biomeMap;

    private void Start()
    {
        rng = new System.Random();

        offsetX = rng.Next(0, 100000);
        offsetZ = rng.Next(0, 100000);

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
        int tileCountPerChunk = Mathf.RoundToInt(chunkWorldSize);  // e.g. 10 tiles per chunk
        float chunkW = chunkWorldSize;
        float chunkD = chunkWorldSize;
        float chunkH = chunkHeight;


        for (int y = 0; y < verticalLayers; y++)
        {
            float depthY = -y * chunkH; // Y is in world units

            int chunkIndexX = 0;
            for (int x = 0; x < worldWidth; x += tileCountPerChunk, chunkIndexX++)
            {
                int chunkIndexZ = 0;
                for (int z = 0; z < worldDepth; z += tileCountPerChunk, chunkIndexZ++)
                {
                    float temp = Mathf.PerlinNoise((x + seed) * temperatureScale,
                                                   (z + seed) * temperatureScale);
                    float moisture = Mathf.PerlinNoise((x + seed + 1000) * moistureScale,
                                                   (z + seed + 1000) * moistureScale);

                    BiomeData data = GetClosestBiome(temp, moisture, depthY);
                    if (data == null) continue;

                    // compute the world‐space center of this chunk
                    Vector3 centerPos = new Vector3(
                        chunkIndexX * chunkW + chunkW / 2f,
                        depthY - chunkH / 2f,
                        chunkIndexZ * chunkD + chunkD / 2f
                    );

                    var volume = Instantiate(
                        biomeVolumePrefab,
                        centerPos,
                        Quaternion.identity,
                        transform
                    );

                    volume.transform.localScale = new Vector3(1f, 1f, 1f);

                    var box = volume.GetComponent<BoxCollider>();
                    if (box != null)
                    {
                        Debug.Log("Deez Nuts");
                        box.size = new Vector3(chunkW, chunkH, chunkD);

                        box.center = Vector3.zero;
                    }

                    if (y == verticalLayers - 1)
                    {
                        float halfH = volumeSize.y * 0.5f;
                        float floorY = centerPos.y - halfH;

                        var floor = Instantiate(seaFloorPrefab, volume.transform);

                        SeafloorGenerator seafloorGenerator = floor.GetComponent<SeafloorGenerator>();
                        if (seafloorGenerator == null)
                        {
                            Debug.LogError("Could not find SeafloorGenerator on Instantiated Object");
                        }

                        seafloorGenerator.Init(seed, x, z, tileSpacing, offsetX, offsetZ, chunkW);

                        // Vector3 volumePos = volume.transform.position; 



                        floor.transform.localScale = Vector3.one;
                        floor.transform.localPosition = Vector3.zero;
                    }



                    // float meshSize = (seafloorGenerator.resolution - 1) * seafloorGenerator.scale;

                    // floor.transform.localScale = new Vector3(meshSize, 1f, meshSize);

                    BiomeVisualManager biomeVisualManager = volume.GetComponent<BiomeVisualManager>();
                    if (biomeVisualManager == null)
                    {
                        Debug.LogError("Could not find BiomeVisualManager on Character component");
                    }



                    // resize the collider to cover exactly chunkW × chunkH × chunkD


                    // assign your biome type
                    var v = volume.GetComponent<BiomeVolume>();
                    if (v != null)
                        v.biomeType = data.biomeType;

                    yield return null;
                }
            }
        }
    }

    
}

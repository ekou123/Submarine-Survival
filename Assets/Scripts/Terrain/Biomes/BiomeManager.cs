using System.Collections.Generic;
using Den.Tools;
using MapMagic.Core;
using MapMagic.Terrains;
using UnityEngine;

[RequireComponent(typeof(MapMagicObject))]
public class BiomeManager : MonoBehaviour
{
    [Header("MapMagic Settings")]
    [Tooltip("The one and only MapMagicObject in your scene")]
    [SerializeField] private MapMagicObject mapMagicObj;

    [Header("Prefabs")]
    public GameObject biomeVolumePrefab;

    [Header("World Settings")]
    public int worldWidth  = 200;
    public int worldDepth  = 1000;
    public int verticalLayers = 4;
    public float chunkHeight   = 20f;
    public List<BiomeData> biomePresets;

    [Header("Noise Settings")]
    public float temperatureScale = 0.01f;
    public float moistureScale    = 0.01f;
    public int   seed             = 1337;

    // internal
    private BiomeData[,] biomeMap;

    void Awake()
    {
        // 1) cache your MapMagicObject (you can also drag‐drop it in the Inspector)
        if (mapMagicObj == null) mapMagicObj = GetComponent<MapMagicObject>();

        // 2) build your 2D biome lookup table once

        // BuildBiomeMap();

        // 3) hook into MapMagic’s tile‐creation event
        // mapMagicObj.tiles.onTileCreated += OnTileCreated;
    }

    void OnDestroy()
    {
        // unhook to avoid leaks
        // mapMagicObj.tiles.onTileCreated -= OnTileCreated;
    }

    // ---------------  
    // 1. Build your biomeMap[x,z]
    // ---------------
    void BuildBiomeMap()
    {
        biomeMap = new BiomeData[worldWidth, worldDepth];
        var rng = new System.Random(seed);
        float offX = rng.Next(0,100000);
        float offZ = rng.Next(0,100000);

        for(int x=0; x<worldWidth; x++)
        for(int z=0; z<worldDepth; z++)
        {
            float temp = Mathf.PerlinNoise((x+offX)*temperatureScale,(z+offZ)*temperatureScale);
            float moist= Mathf.PerlinNoise((x+offX+1000)*moistureScale,(z+offZ+1000)*moistureScale);
            biomeMap[x,z] = GetClosestBiome(temp,moist,0f);
        }
    }

    BiomeData GetClosestBiome(float t, float m, float depthY)
    {
        BiomeData best = null;
        float    minD = float.MaxValue;
        foreach(var b in biomePresets)
        {
            if (depthY>b.minDepth || depthY<b.maxDepth) continue;
            float dT = b.temperature - t;
            float dM = b.moisture    - m;
            float d  = dT*dT + dM*dM;
            if (d<minD) { minD=d; best=b; }
        }
        return best;
    }

    // ---------------  
    // 2. Called for each tile MM generates
    // ---------------
    private void OnTileCreated(Coord coord, TerrainTile tile)
    {
        // the actual Terrain GameObject
        var go      = tile.gameObject;
        var terrain = go.GetComponent<Terrain>();
        if (terrain==null) return;

        // size in world‐units of that tile
        Vector3 size = terrain.terrainData.size;

        // world‐space X,Z center for this tile
        Vector3 centerXZ = go.transform.position + new Vector3(size.x*0.5f, 0, size.z*0.5f);

        // for each vertical slice (layer)
        for(int y=0; y<verticalLayers; y++)
        {
            float depthY = -y * chunkHeight;
            float sampleX = (coord.x * size.x) + size.x*0.5f;
            float sampleZ = (coord.z * size.z) + size.z*0.5f;

            float temp = Mathf.PerlinNoise(sampleX*temperatureScale, sampleZ*temperatureScale);
            float moist= Mathf.PerlinNoise((sampleX+1000)*moistureScale, (sampleZ+1000)*moistureScale);

            var data = GetClosestBiome(temp, moist, depthY);
            if (data==null) continue;


            
            // spawn your biome volume at the correct world Y
            Vector3 spawnPos = new Vector3(centerXZ.x, depthY - chunkHeight * 0.5f, centerXZ.z);
            var volume = Instantiate(biomeVolumePrefab, spawnPos, Quaternion.identity, go.transform);

                // set up its collider
            var box = volume.GetComponent<BoxCollider>();
            box.size = new Vector3(size.x, chunkHeight, size.z);
            box.center = Vector3.zero;
                
            // assign your biome data
            var v = volume.GetComponent<BiomeVolume>();
            if (v!=null) v.biomeData = data;
            
            

            
        }
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MapMagic;
// using MapMagic.Core;   // <-- brings in Vector2i

// public class MapMagicBiomeManager : MonoBehaviour
// {
//     [Header("MapMagic Settings")]
//     public MapMagicObject mapMagic;            // drag in your MapMagic component
//     public int worldChunksX = 4;         // how many chunks along X
//     public int worldChunksZ = 4;         // how many along Z

//     [Header("Chunk Prefabs")]
//     public GameObject biomeVolumePrefab; // your chunk “container”
//     public GameObject seaFloorPrefab;    // floor prefab with SeafloorGenerator

//     [Header("Chunk Dimensions")]
//     public float chunkWorldSize = 500f;  // must match MapMagic.Tile Size
//     public float chunkHeight     = 20f;  // your vertical layer height
//     public int   verticalLayers  = 4;    // how many layers per XZ chunk
//     public float waterLevel      = 0f;   // Y at which to place the floor

//     [Header("Biome Noise Settings")]
//     public List<BiomeData> biomePresets; // as before
//     public int    seed             = 1337;
//     public float  temperatureScale = 0.01f;
//     public float  moistureScale    = 0.01f;

//     // internal:
//     private BiomeData[,] globalBiomeMap;
//     private float   offsetX, offsetZ;
//     private float   biomeCellSize = 1f;
//     private Vector2 biomeMapOrigin = Vector2.zero;

//     void Start()
//     {
        
//         // 1) prep noise & global map
//         var rng = new System.Random(seed);
//         offsetX = rng.Next(0,100000);
//         offsetZ = rng.Next(0,100000);
//         BuildGlobalBiomeMap();

//         // 2) tell MapMagic to load exactly the chunks we care about
//         for(int x=0; x<worldChunksX; x++)
//             for(int z=0; z<worldChunksZ; z++)
//                 //mapMagic.AddTile(new Vector2Int(x,z));

//         // 3) hook MapMagic’s tile-spawn event
//         //mapMagic.onAddLateActions += OnTileSpawned;
//     }

//     void OnDestroy()
//     {
//         mapMagic.onAddLateActions -= OnTileSpawned;
//     }

//     // This gets called once per chunk, whenever MapMagic has finished generating it.
//     void OnTileSpawned(Vector2Int coord)
//     {
//         // center‐of‐chunk in world-space:
//         Vector3 chunkOrigin = new Vector3(
//             coord.x * chunkWorldSize,
//             0,
//             coord.y * chunkWorldSize
//         );

//         // spawn your vertical layers
//         for(int layer=0; layer<verticalLayers; layer++)
//             SpawnLayer(coord, chunkOrigin, layer);
//     }

//     void SpawnLayer(Vector2Int coord, Vector3 chunkOrigin, int layer)
//     {
//         // center‐pos of this BiomeVolume
//         float yCenter = -(layer * chunkHeight + chunkHeight*0.5f);
//         Vector3 volPos = chunkOrigin + new Vector3(chunkWorldSize*0.5f, yCenter, chunkWorldSize*0.5f);

//         // 1) instantiate the “chunk container”
//         var volume = Instantiate(biomeVolumePrefab, volPos, Quaternion.identity, transform);
//         var box    = volume.GetComponent<BoxCollider>();
//         box.size   = new Vector3(chunkWorldSize, chunkHeight, chunkWorldSize);
//         box.center = Vector3.zero;

//         // 2) pick its biome
//         BiomeData data = SampleGlobalBiome(volPos.x, volPos.z, -layer*chunkHeight);
//         volume.GetComponent<BiomeVolume>().biomeData = data;

//         // 3) for the bottom layer, spawn the SeafloorGenerator floor
//         if (layer == 0)
//         {
//             var floor = Instantiate(seaFloorPrefab, volume.transform);
//             floor.transform.position = new Vector3(volPos.x, waterLevel, volPos.z);
//             floor.transform.localScale  = Vector3.one;

//             var gen = floor.GetComponent<SeafloorGenerator>();
//             gen.Init(
//                 seed,
//                 coord.x, coord.y,
//                 chunkWorldSize,
//                 offsetX, offsetZ,
//                 globalBiomeMap,
//                 biomeCellSize,
//                 biomeMapOrigin
//             );
//             gen.Build();
//         }
//     }

//     // Re-use your Perlin-based map logic exactly as before:
//     void BuildGlobalBiomeMap()
//     {
//         int w = worldChunksX * Mathf.RoundToInt(chunkWorldSize/biomeCellSize);
//         int d = worldChunksZ * Mathf.RoundToInt(chunkWorldSize/biomeCellSize);
//         globalBiomeMap = new BiomeData[w, d];
//         var rng = new System.Random(seed);
//         float ox = rng.Next(0,100000), oz = rng.Next(0,100000);

//         for(int x=0; x<w; x++)
//         for(int z=0; z<d; z++)
//         {
//             float temp = Mathf.PerlinNoise((x+ox)*temperatureScale, (z+oz)*temperatureScale);
//             float moist= Mathf.PerlinNoise((x+ox+1000)*moistureScale, (z+oz+1000)*moistureScale);
//             globalBiomeMap[x,z] = GetClosestBiome(temp,moist, 0f);
//         }
//     }

//     BiomeData SampleGlobalBiome(float worldX, float worldZ, float depthY)
//     {
//         int ix = Mathf.Clamp( Mathf.FloorToInt((worldX-biomeMapOrigin.x)/biomeCellSize), 0, globalBiomeMap.GetLength(0)-1 );
//         int iz = Mathf.Clamp( Mathf.FloorToInt((worldZ-biomeMapOrigin.y)/biomeCellSize), 0, globalBiomeMap.GetLength(1)-1 );
//         return globalBiomeMap[ix, iz];
//     }

//     BiomeData GetClosestBiome(float temp, float moisture, float depthY)
//     {
//         BiomeData best = null;
//         float    minD = float.MaxValue;
//         foreach(var b in biomePresets)
//         {
//             if (depthY> b.minDepth || depthY< b.maxDepth) continue;
//             float dT = b.temperature - temp, dM = b.moisture - moisture;
//             float dist = dT*dT + dM*dM;
//             if (dist<minD) { minD=dist; best=b; }
//         }
//         return best;
//     }
// }

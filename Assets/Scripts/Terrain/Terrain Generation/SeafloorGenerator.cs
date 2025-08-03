using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SeafloorGenerator : MonoBehaviour
{
    [Header("Mesh Settings")]
    public int resolution = 100;    // verts per side

    // these get blended per-vertex:
    private float offsetX, offsetZ; 
    private BiomeData[,] globalBiomeMap;
    private float biomeCellSize;    // world units per cell in globalBiomeMap
    private Vector2 biomeMapOrigin; // world coords of (0,0) in globalBiomeMap

    [Header("Ocean Slope Settings")]
    public float globalSlopeStart     =  0f;   // where the drop begins
    public float globalSlopeStrength  =  0.5f; // tweak this in the inspector

    // derived per-chunk:
    private float _meshSize;        // = chunkWorldSize
    private MeshFilter _mf;

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
    }

    public void Init(
        int seed,
        int chunkX,
        int chunkZ,
        float chunkWorldSize,
        float offsetX,
        float offsetZ,
        BiomeData[,] globalBiomeMap,
        float biomeCellSize,
        Vector2 biomeMapOrigin
    ) {
        this.offsetX         = offsetX;
        this.offsetZ         = offsetZ;
        this.globalBiomeMap  = globalBiomeMap;
        this.biomeCellSize   = biomeCellSize;
        this.biomeMapOrigin  = biomeMapOrigin;
        this._meshSize       = chunkWorldSize;
    }

    public void Build()
    {
        StartCoroutine(BuildMeshCoroutine());
    }

    IEnumerator BuildMeshCoroutine()
    {
        int N = resolution;
        int vertCount = N * N;
        var verts = new Vector3[vertCount];
        var uvs   = new Vector2[vertCount];
        var tris  = new int[(N - 1) * (N - 1) * 6];

        float half = _meshSize * 0.5f;

        // 1) vertices + UVs
        for (int z = 0; z < N; z++)
        {
            for (int x = 0; x < N; x++)
            {
                int i = x + z * N;
                // local coord
                Vector3 local = new Vector3(
                    x * (_meshSize / (N - 1)) - half,
                    0,
                    z * (_meshSize / (N - 1)) - half
                );
                Vector3 worldPos = transform.position + local;

                // —— lookup & blend 4 biomes —— 
                float fx = (worldPos.x - biomeMapOrigin.x) / biomeCellSize;
                float fz = (worldPos.z - biomeMapOrigin.y) / biomeCellSize;
                int ix = Mathf.Clamp(Mathf.FloorToInt(fx), 0, globalBiomeMap.GetLength(0) - 2);
                int iz = Mathf.Clamp(Mathf.FloorToInt(fz), 0, globalBiomeMap.GetLength(1) - 2);
                float u = fx - ix, v = fz - iz;

                BiomeData b00 = globalBiomeMap[ix,   iz];
                BiomeData b10 = globalBiomeMap[ix+1, iz];
                BiomeData b01 = globalBiomeMap[ix,   iz+1];
                BiomeData b11 = globalBiomeMap[ix+1, iz+1];

                float w00 = (1-u)*(1-v), w10 = u*(1-v), w01 = (1-u)*v, w11 = u*v;

                // blend parameters
                float noiseFreq     = b00.noiseFrequency * w00 + b10.noiseFrequency * w10 + b01.noiseFrequency * w01 + b11.noiseFrequency * w11;
                float heightMul     = b00.heightMultiplier * w00 + b10.heightMultiplier * w10 + b01.heightMultiplier * w01 + b11.heightMultiplier * w11;
    
                float blendedSlopeStart   = b00.slopeStartZ   * w00 + b10.slopeStartZ   * w10 + b01.slopeStartZ   * w01 + b11.slopeStartZ   * w11;
                float blendedSlopeStrength= b00.slopeStrength * w00 + b10.slopeStrength * w10 + b01.slopeStrength * w01 + b11.slopeStrength * w11;

                // 2) Compute your height
                float noise = Mathf.PerlinNoise(
                      (worldPos.x + offsetX) * noiseFreq,
                      (worldPos.z + offsetZ) * noiseFreq
                  ) * heightMul;

                // local biome slope
                float localSlope = Mathf.Max(0, worldPos.z - blendedSlopeStart) 
                       * blendedSlopeStrength;

                // optional: keep a global ocean‐floor drop
                float globalDrop = Mathf.Max(0, worldPos.z - globalSlopeStart) 
                       * globalSlopeStrength;

                float h = noise + localSlope + globalDrop;

                verts[i] = local + Vector3.down * h;
                uvs[i]   = new Vector2(x / (float)(N - 1), z / (float)(N - 1));
            }
            yield return null;
        }

        // 2) build tris
        int t = 0;
        for (int z = 0; z < N - 1; z++)
        {
            for (int x = 0; x < N - 1; x++)
            {
                int i = x + z * N;
                tris[t++] = i;
                tris[t++] = i + N;
                tris[t++] = i + 1;
                tris[t++] = i + 1;
                tris[t++] = i + N;
                tris[t++] = i + N + 1;
            }
            yield return null;
        }

        // 3) finalize
        var mesh = new Mesh {
            indexFormat = vertCount > 65000
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16
        };
        mesh.vertices  = verts;
        mesh.triangles = tris;
        mesh.uv        = uvs;
        mesh.RecalculateNormals();
        _mf.mesh       = mesh;
    }
}
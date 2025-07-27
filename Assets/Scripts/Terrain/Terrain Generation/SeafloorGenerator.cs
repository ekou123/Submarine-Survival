using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SeafloorGenerator : MonoBehaviour
{
    [Header("Mesh Settings")]
    public int resolution = 100;   // verts per side
    public float scale = 1f;    // world units between verts
    public float heightMul = 10f;
    public float noiseFreq = 0.1f;
    public Vector2 noiseOffset;
    [SerializeField] private float chunkWorldSize = 500f;
    [SerializeField] private float chunkHeight = 500f;

    [Header("Seafloor Settings")]
    private int seed;
    private Vector2 chunkOrigin;
    private float tileSpacing;
    private float offsetX;
    private float offsetZ;

    [HideInInspector] public int chunkX, chunkZ;

    MeshFilter _mf;
    float _meshSize;   // = (resolution-1)*scale

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        //_meshSize = (resolution - 1) * scale;
    }

    void Start()
    {
        // pass the chunk's world offset into the shader
        var rend = GetComponent<MeshRenderer>();
        var mat = rend.material;
        mat.SetFloat("_NoiseScale", noiseFreq);
        mat.SetFloat("_HeightMul", heightMul);
        // world-space offset for noise tiling:
        mat.SetVector("_NoiseOffset", new Vector4(
            chunkX * _meshSize + transform.position.x,
            chunkZ * _meshSize + transform.position.z,
            0, 0
        ));

        StartCoroutine(BuildMeshCoroutine());
    }

    IEnumerator BuildMeshCoroutine()
    {
        int N = resolution;
        int vertCount = N * N;
        var verts = new Vector3[vertCount];
        var uvs = new Vector2[vertCount];
        var tris = new int[(N - 1) * (N - 1) * 6];

        // half-size so we can center the pivot
        float half = _meshSize * 0.5f;

        // 1) build vertices & UVs in LOCAL space (centered)
        for (int z = 0; z < N; z++)
        {
            for (int x = 0; x < N; x++)
            {
                int i = x + z * N;
                // local position ranges from -half .. +half
                Vector3 local = new Vector3(
                    x * scale - half,
                    0,
                    z * scale - half
                );

                // for noise, sample in WORLD space:
                Vector3 worldSample = transform.position + local;
                float h = Mathf.PerlinNoise(
                    (worldSample.x + noiseOffset.x) * noiseFreq,
                    (worldSample.z + noiseOffset.y) * noiseFreq
                ) * heightMul;

                verts[i] = local + Vector3.down * h;
                uvs[i] = new Vector2(x / (float)(N - 1), z / (float)(N - 1));
            }
            yield return null;
        }

        // 2) build triangles
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

        // 3) build mesh
        var mesh = new Mesh
        {
            indexFormat = vertCount > 65000
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16
        };
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        _mf.mesh = mesh;

        Debug.Log($"[Mesh] scale: {scale}, meshSize: {_meshSize}, position: {transform.position}");
    }

    public void Init(int seed, int chunkX, int chunkZ, float spacing, float offsetX, float offsetZ, float chunkWorldSize)
    {
        // store seed & offsets
        this.seed = seed;
        this.chunkOrigin = new Vector2(chunkX, chunkZ);
        this.tileSpacing = spacing;
        this.offsetX = offsetX;
        this.offsetZ = offsetZ;

        this.scale = chunkWorldSize / (resolution - 1);
        _meshSize = chunkWorldSize;

    }

// #if UNITY_EDITOR
//     void OnDrawGizmos()
//     {
//         // compute the total span of your mesh:
//         float meshSize = (resolution - 1) * scale;

//         // pick a color
//         Gizmos.color = Color.cyan;

//         // draw a wire‐frame cube at this GameObject’s position,
//         // centered on the XZ grid, 1 unit tall
//         Vector3 center = transform.position;
//         Vector3 size = new Vector3(meshSize, 1f, meshSize);
//         Gizmos.DrawWireCube(center, size);
//     }
// #endif

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_meshSize, 1, _meshSize));
    }

}
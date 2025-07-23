using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SeafloorGenerator : MonoBehaviour
{
    public int    resolution = 100;
    public float  scale      = 1f;
    public float  heightMul  = 10f;
    public float  noiseFreq  = 0.1f;
    public Vector2 noiseOffset;
    public int    chunkX, chunkZ;

    private MeshFilter _mf;

    void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        var mat      = renderer.material;
        mat.SetFloat("_NoiseScale", noiseFreq);
        mat.SetFloat("_HeightMul",  heightMul);
        mat.SetVector("_NoiseOffset", new Vector4(
        chunkX * (resolution-1) * scale,
        chunkZ * (resolution-1) * scale,
        0,0
    ));
    
    StartCoroutine(BuildMeshCoroutine());
    }

    IEnumerator BuildMeshCoroutine()
    {
        int N = resolution;
        int vertCount = N * N;
        Vector3[] verts = new Vector3[vertCount];
        Vector2[] uvs   = new Vector2[vertCount];
        int triCount = (N-1)*(N-1)*6;
        int[] tris = new int[triCount];

        float baseX = chunkX * (N-1) * scale;
        float baseZ = chunkZ * (N-1) * scale;

        // 1) Vert rows
        for (int z = 0; z < N; z++)
        {
            for (int x = 0; x < N; x++)
            {
                int i = x + z * N;
                float worldX = baseX + x * scale;
                float worldZ = baseZ + z * scale;
                float h = Mathf.PerlinNoise((worldX + noiseOffset.x) * noiseFreq,
                                            (worldZ + noiseOffset.y) * noiseFreq)
                          * heightMul;
                verts[i] = new Vector3(worldX, -h, worldZ);
                uvs[i]   = new Vector2(x / (float)(N-1), z / (float)(N-1));
            }
            // yield each row so the main thread can render
            yield return null;
        }

        // 2) Tri rows
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

        // 3) Build mesh
        Mesh m = new Mesh();
        m.indexFormat = vertCount > 65000 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
        m.vertices  = verts;
        m.triangles = tris;
        m.uv        = uvs;
        m.RecalculateNormals();

        _mf.mesh = m;
    }
}
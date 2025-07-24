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

    private void Awake() {
        _mf = GetComponent<MeshFilter>();
    }

    void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        var mat      = renderer.material;
        mat.SetFloat("_NoiseScale", noiseFreq);
        mat.SetFloat("_HeightMul",  heightMul);
        mat.SetVector
        ("_NoiseOffset",
        new Vector4(
        chunkX * (resolution - 1) * scale,
        chunkZ * (resolution - 1) * scale,
        0, 0));

        


    
    StartCoroutine(BuildMeshCoroutine());
    }

    IEnumerator BuildMeshCoroutine()
    {
        int N = resolution;
        int vertCount = N * N;
        var verts = new Vector3[vertCount];
        var uvs = new Vector2[vertCount];
        var tris = new int[(N - 1) * (N - 1) * 6];

        // 1) build vertices & UVs in LOCAL space
        for (int z = 0; z < N; z++)
        {
            for (int x = 0; x < N; x++)
            {
                int i = x + z * N;
                Vector3 localPos = new Vector3(x * scale, 0, z * scale);

                // WORLD‐space sample position:
                Vector3 worldSample = transform.position + localPos;
                float h = Mathf.PerlinNoise(
                    (worldSample.x + noiseOffset.x) * noiseFreq,
                    (worldSample.z + noiseOffset.y) * noiseFreq
                ) * heightMul;

                verts[i] = localPos + Vector3.down * h;
                uvs[i] = new Vector2(x / (float)(N - 1), z / (float)(N - 1));
            }
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
        m.vertices = verts;
        m.triangles = tris;
        m.uv = uvs;
        m.RecalculateNormals();

        _mf.mesh = m;
        
        
    }
}
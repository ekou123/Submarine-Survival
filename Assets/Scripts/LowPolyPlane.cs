using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LowPolyPlane : MonoBehaviour
{
    [Tooltip("Vertices per side")]
    public int resolution = 32;
    [Tooltip("World-space distance between verts")]
    public float scale = 1f;

    void Start()
    {
        int N = resolution;
        Vector3[] verts = new Vector3[N * N];
        Vector2[] uvs   = new Vector2[verts.Length];
        int[]     tris  = new int[(N - 1) * (N - 1) * 6];

        // 1) Build vertices + UVs
        for (int z = 0; z < N; z++)
        for (int x = 0; x < N; x++)
        {
            int i = x + z * N;
            verts[i] = new Vector3(x * scale, 0, z * scale);
            uvs[i]   = new Vector2(x / (float)(N - 1), z / (float)(N - 1));
        }

        // 2) Build triangles
        int t = 0;
        for (int z = 0; z < N - 1; z++)
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

        // 3) Assign to mesh
        var mesh = new Mesh {
            vertices  = verts,
            triangles = tris,
            uv        = uvs
        };
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
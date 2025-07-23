using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SeafloorGenerator : MonoBehaviour
{
    public int    resolution = 50;
    public float  scale      = 1f;
    public float  heightMul  = 10f;
    public float  noiseFreq  = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        BuildMesh();
    }

    private void BuildMesh()
    {
        int vertsPerLine = resolution;
        Vector3[] verts   = new Vector3[vertsPerLine*vertsPerLine];
        Vector2[] uvs     = new Vector2[verts.Length];
        int[]     tris    = new int[(vertsPerLine-1)*(vertsPerLine-1)*6];

        // 1) generate vertices + UVs
        for(int z=0; z<vertsPerLine; z++)
        for(int x=0; x<vertsPerLine; x++)
        {
            int i = x + z*vertsPerLine;
            float height = Mathf.PerlinNoise(x*noiseFreq, z*noiseFreq) * heightMul;
            verts[i] = new Vector3(x*scale, -height, z*scale);
            uvs[i]   = new Vector2((float)x/vertsPerLine, (float)z/vertsPerLine);
        }

        // 2) generate triangles
        int t=0;
        for(int z=0; z<vertsPerLine-1; z++)
        for(int x=0; x<vertsPerLine-1; x++)
        {
            int i = x + z*vertsPerLine;

            tris[t++] = i;
            tris[t++] = i + vertsPerLine;
            tris[t++] = i + 1;

            tris[t++] = i + 1;
            tris[t++] = i + vertsPerLine;
            tris[t++] = i + vertsPerLine + 1;
        }

        // 3) assign to mesh
        Mesh mesh = new Mesh();
        mesh.vertices  = verts;
        mesh.triangles = tris;
        mesh.uv        = uvs;
        mesh.RecalculateNormals();

        var mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
    }
}

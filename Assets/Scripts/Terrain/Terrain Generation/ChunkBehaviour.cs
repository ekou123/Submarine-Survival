using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBehaviour : MonoBehaviour
{
    public GameObject chunkPrefab;
    public float noiseFreq;
    public float heightMul;
    public float chunkSize = 10;
    [SerializeField] int mapWidthInChunks = 1;
    [SerializeField] int mapDepthInChunks = 1;

    private void Start()
    {
        // for (int cx = 0; cx < mapWidthInChunks; cx++)
        //     for (int cz = 0; cz < mapDepthInChunks; cz++)
        //     {
        //         var go = Instantiate(seaFloorChunkPrefab);
        //         var gen = go.GetComponent<SeafloorGenerator>();
        //         gen.chunkX = cx;
        //         gen.chunkZ = cz;
        //     }
    }
    


    public void SpawnChunk(int cx, int cz)
    {
        // 1) Instantiate your low-poly plane with the SG_SeafloorDisplacement material
        var chunk = Instantiate(
            chunkPrefab,
            new Vector3(cx*chunkSize, 0, cz*chunkSize),
            Quaternion.identity,
            transform
        );

        // 2) Grab its MeshRenderer
        var renderer = chunk.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogError("Chunk prefab needs a MeshRenderer!");
            return;
        }

        // 3) Tweak the shader properties right here
        var mat = renderer.material;                // this creates an instance if needed
        mat.SetFloat("_NoiseScale", noiseFreq);
        mat.SetFloat("_HeightMul",   heightMul);
        
        // e.g. use chunkX/Z to offset the noise so tiles line up seamlessly:
        float offsetX = cx * chunkSize;
        float offsetZ = cz * chunkSize;
        mat.SetVector("_NoiseOffset", new Vector4(offsetX, offsetZ, 0, 0));
    }
}


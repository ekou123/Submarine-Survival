using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public ChunkBehaviour spawner;    // reference to your component with SpawnChunk()
    public int mapWidthInChunks  = 10;
    public int mapDepthInChunks  = 10;

    void Start()
    {
        // Loop through every chunk coord you want
        for (int x = 0; x < mapWidthInChunks; x++)
        for (int z = 0; z < mapDepthInChunks; z++)
        {
                spawner.SpawnChunk(x, z);
        }
    }
}

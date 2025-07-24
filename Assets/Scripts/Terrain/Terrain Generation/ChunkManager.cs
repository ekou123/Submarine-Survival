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
    }
}

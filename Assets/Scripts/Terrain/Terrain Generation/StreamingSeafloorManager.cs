using System.Collections.Generic;
using UnityEngine;

public class StreamingSeafloorManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject chunkPrefab;    // your 32×32 plane with the displacement shader
    public int        viewRadius     = 2;     // how many chunks out in X/Z to keep
    public float      chunkSize      = 5f;  // world‐space width/depth of each chunk
    public float      scale = 1f;
    public int        resolution = 100;
    public float      noiseFreq = 0.1f;
    public float      heightMul = 10f;

    float meshSize;

    [Header("Player")]
    public Transform  player;         // drag in your Camera or Character transform

    // internal state
    Dictionary<ChunkCoord, GameObject> _loadedChunks = new();
    Queue<GameObject>                  _pool         = new();
    public void Setup(Transform playerTransform)
    {
        player = playerTransform;

        // 1) Pre-allocate (viewRadius*2+1)^2 chunk GameObjects
        int poolSize = (2*viewRadius + 1) * (2*viewRadius + 1);
        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(chunkPrefab, transform);

            var gen = go.GetComponent<SeafloorGenerator>();
            if (gen == null)
            {
                Debug.LogError("Could not find SeafloorGenerator on Instantiated Chunk Prefab");
                return;
            }

            gen.resolution = resolution;
            gen.scale = scale;
            gen.noiseFreq = noiseFreq;
            gen.heightMul = heightMul;
            gen.noiseOffset = Vector2.zero;

            meshSize = (resolution - 1) * scale;

            go.SetActive(false);
            _pool.Enqueue(go);
        }
        // 2) Load initial set
        UpdateLoadedChunks();
    }

    void Update()
    {
        UpdateLoadedChunks();
    }

    void UpdateLoadedChunks()
    {
        if (player == null)
        {
            return;
        }

        // Determine the chunk coords the player is standing in
        int pcx = Mathf.FloorToInt(player.position.x / chunkSize);
        int pcz = Mathf.FloorToInt(player.position.z / chunkSize);

        //  A) Spawn any missing chunks in the square [–viewRadius … +viewRadius]
        for (int dz = -viewRadius; dz <= viewRadius; dz++)
        for (int dx = -viewRadius; dx <= viewRadius; dx++)
        {
            var coord = new ChunkCoord(pcx + dx, pcz + dz);
            if (_loadedChunks.ContainsKey(coord)) continue;
            if (_pool.Count == 0) continue;  // (shouldn’t happen)

            // pull one from the pool
            var go = _pool.Dequeue();
            _loadedChunks[coord] = go;

            // position it in world
            go.transform.position = new Vector3(
                coord.x * meshSize,
                0,
                coord.z * meshSize
            );
            go.SetActive(true);

            // set up its shader parameters via MaterialPropertyBlock
            var rend = go.GetComponent<Renderer>();
            var mpb  = new MaterialPropertyBlock();
            mpb.SetFloat("_NoiseScale", noiseFreq);
            mpb.SetFloat("_HeightMul",   heightMul);
            mpb.SetVector("_NoiseOffset", new Vector4(
                coord.x * meshSize,
                coord.z * meshSize,
                0, 0
            ));
            rend.SetPropertyBlock(mpb);
        }

        //  B) Unload any chunks that moved outside the radius
        var toRemove = new List<ChunkCoord>();
        foreach (var kv in _loadedChunks)
        {
            var c = kv.Key;
            if (Mathf.Abs(c.x - pcx) > viewRadius ||
                Mathf.Abs(c.z - pcz) > viewRadius)
            {
                toRemove.Add(c);
            }
        }
        foreach (var c in toRemove)
        {
            var go = _loadedChunks[c];
            _loadedChunks.Remove(c);
            go.SetActive(false);
            _pool.Enqueue(go);
        }
    }

    // simple int‐pair key for Dictionary
    struct ChunkCoord
    {
        public int x, z;
        public ChunkCoord(int x, int z) { this.x = x; this.z = z; }
        public override bool Equals(object o)
            => (o is ChunkCoord cc) && cc.x == x && cc.z == z;
        public override int GetHashCode() => x * 397 ^ z;
    }
}

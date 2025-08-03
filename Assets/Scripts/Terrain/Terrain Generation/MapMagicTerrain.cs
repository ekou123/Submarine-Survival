using System.Collections;
using System.Collections.Generic;
using MapMagic.Core;
using MapMagic.Terrains;
using UnityEngine;

[RequireComponent(typeof(MapMagicObject))]
public class MapMagicTerrain : MonoBehaviour
{
    [Tooltip("Must match your Graph's Tile Size")]
    [SerializeField] private float tileSize = 500f;
    [SerializeField] private float boxHeight = 50f;

    private MapMagicObject mmObj;

    void OnEnable()
    {
        mmObj = GetComponent<MapMagicObject>();
        mmObj.tiles.onTileCreated += HandleTileCreated;
    }

    void OnDisable()
    {
        mmObj.tiles.onTileCreated -= HandleTileCreated;
    }

    private void HandleTileCreated(Den.Tools.Coord coord, TerrainTile tile)
    {
        var go = tile.gameObject;                     // ← use tile.obj
        var terr = go.GetComponent<Terrain>();
        var size = terr.terrainData.size;

        var box = go.AddComponent<BoxCollider>();
        box.center = new Vector3(size.x/2, -boxHeight/2, size.z/2);
        box.size   = new Vector3(size.x, boxHeight, size.z);
    }
}

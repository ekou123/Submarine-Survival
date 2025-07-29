using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewBiome", menuName = "World/Biome")]
public class BiomeData : ScriptableObject
{
    public Biome biome;

    [Header("Environment")]
    public int resolution = 100;   // verts per side
    public float scale = 1f;    // world units between verts
    public float heightMultiplier = 10f;
    public float noiseFrequency = 0.1f;
    public Color fogColor;
    public float  fogDensity;
    public Color  ambientColor;
    public float slopeStrength;
    public float slopeStartZ;

    [Header("Water")]
    public Material waterMaterialOverride;
    public Color    waterSurfaceColor;
    public Color    waterUnderColor;

    [Header("Post‐Process")]
    public VolumeProfile postProcessProfile;

    [Range(0f, 1f)]
    public float temperature;

    [Range(0f, 1f)]
    public float moisture;

    public Color debugColor;

    [Header("Depth Range")]
    public float minDepth;
    public float maxDepth;

    [Header("Biome Prefabs")]
    public GameObject[] terrainPrefabs;

    

    public void Setup()
    {
        biome = new Biome();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "World/Biome")]
public class BiomeData : ScriptableObject
{
    public BiomeType biomeType;

    [Range(0f, 1f)]
    public float temperature;

    [Range(0f, 1f)]
    public float moisture;

    public Color debugColor;

    [Header("Biome Prefabs")]
    public GameObject[] terrainPrefabs;
}

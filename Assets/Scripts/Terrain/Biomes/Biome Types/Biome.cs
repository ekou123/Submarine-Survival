using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType
{
    Shallow,
    Deep,
    Test,
}

public class Biome
{
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

    public void Setup(float temp, float moist, Color debugC, float minDep, float maxDep)
    {
        temperature = temp;
        moisture = moist;
        debugColor = debugC;
        minDepth = minDep;
        maxDepth = maxDep;
    }
        
}

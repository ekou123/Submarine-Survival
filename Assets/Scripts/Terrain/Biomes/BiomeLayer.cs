using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Biome Layer", menuName = "World/Biome Layer")]
public class BiomeLayer : ScriptableObject
{
    [Header("Which Biome?")]
    public BiomeData biomeData;

    [Header("Paint or Import mask here")]
    public Texture2D blendMask;

    [HideInInspector]
    public float[,] heightMap;

}

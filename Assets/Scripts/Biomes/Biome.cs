using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType
{
    Shallow,
    Deep
}

public class Biome
{
    public BiomeType biomeType;
    public Color debugColor;
    public float temperature;
    public float moisture;
}

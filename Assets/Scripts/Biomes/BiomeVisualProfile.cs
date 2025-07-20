using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "World/Biome Visual Profile")]
public class BiomeVisualProfile : ScriptableObject
{
    public BiomeType biomeType;

    [Header("Fog/Ambient")]
    public Color fogColor = Color.white;
    public float fogDensity = 0.02f;
    public Color ambientColor = Color.white;

    [Header("Water Material")]
    public Material waterMaterialOverride;
    public Color waterSurfaceColor = Color.white;
    public Color waterUnderColor = Color.white;

    [Header("Post-Process (URP/HDRP)")]
    public VolumeProfile postProcessProfile;
}

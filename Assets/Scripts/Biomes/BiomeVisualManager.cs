using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BiomeVisualManager : MonoBehaviour
{
    public List<BiomeVisualProfile> profiles;
    public Volume globalVolume;
    public Light directionalLight;
    public BiomeDetector detector;
    public WaterSurface _waterSurface;
    Dictionary<BiomeType, BiomeVisualProfile> _lookup;


    private void Awake()
    {
        
    }

    public void Setup(Character character)
    {
        // 1) Find the local BiomeDetector on the player
        detector = character.GetComponentInChildren<BiomeDetector>();
        if (detector == null)
        {
            Debug.LogError("Could not find BiomeDetector on Character!");
            return;
        }

        // 2) Auto‐find your global Volume and Directional Light
        globalVolume    = FindObjectOfType<Volume>();
        directionalLight = GameObject.FindWithTag("Sun")?.GetComponent<Light>();

        // 3) Find the HDRP WaterSurface in the Scene
        _waterSurface = FindObjectOfType<WaterSurface>();
        if (_waterSurface == null)
        {
            Debug.LogError("No HDRP WaterSurface found in scene!");
            return;
        }

        // 4) Build lookup table
        _lookup = new Dictionary<BiomeType, BiomeVisualProfile>();
        foreach (var p in profiles)
            _lookup[p.biomeType] = p;

        // 5) Subscribe to biome‐change events
        detector.OnBiomeChanged += ApplyProfile;
    }

    void ApplyProfile(BiomeType newBiome)
    {
        if (!_lookup.TryGetValue(newBiome, out var prof))
            return;

        // --- 1) Fog & ambient ---
        RenderSettings.fogColor   = prof.fogColor;
        RenderSettings.fogDensity = prof.fogDensity;
        RenderSettings.ambientLight = prof.ambientColor;

        // --- 2) WaterSurface tweaks ---
        // a) swap whole material if provided
        if (prof.waterMaterialOverride != null)
            _waterSurface.customMaterial = prof.waterMaterialOverride;

        // b) tint refraction and scattering
        _waterSurface.refractionColor = prof.waterSurfaceColor;
        _waterSurface.scatteringColor = prof.waterUnderColor;

        // (You can also tweak:)
        // _waterSurface.absorptionDistance = prof.absorptionDistance;
        // _waterSurface.foamColor = prof.foamColor;
        // _waterSurface.caustics = prof.enableCaustics;

        // --- 3) Post‐process swap ---
        if (globalVolume != null && prof.postProcessProfile != null)
            globalVolume.profile = prof.postProcessProfile;

        // --- 4) Sun tint (optional) ---
        if (directionalLight != null)
            directionalLight.color = prof.fogColor * 0.8f;
    }
}

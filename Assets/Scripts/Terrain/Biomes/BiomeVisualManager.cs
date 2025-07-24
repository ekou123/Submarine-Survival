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

    Character _character;
    Dictionary<BiomeType, BiomeVisualProfile> _lookup;


    private void Awake()
    {
        
    }
    
    private void Start() {
        StartCoroutine(InitWhenCharacterExists());
    }

    IEnumerator InitWhenCharacterExists()
    {
        // spinlock until Character.Local is non‐null
        while (Character.Local == null)
            yield return null;

        _character = Character.Local;
        detector   = _character.GetComponentInChildren<BiomeDetector>();
        if (detector == null)
        {
            Debug.LogError("No BiomeDetector on local Character!");
            yield break;
        }

        // now do the rest of your Setup logic:
        globalVolume     = FindObjectOfType<Volume>();
        directionalLight = GameObject.FindWithTag("Sun")?.GetComponent<Light>();
        _waterSurface    = FindObjectOfType<WaterSurface>();

        // build lookup table
        _lookup = new Dictionary<BiomeType,BiomeVisualProfile>();
        foreach (var p in profiles) 
            _lookup[p.biomeType] = p;

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

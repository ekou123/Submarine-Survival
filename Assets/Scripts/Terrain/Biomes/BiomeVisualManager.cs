using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BiomeVisualManager : MonoBehaviour
{
    public BiomeDetector detector;
    public Volume        globalVolume;
    public Light         directionalLight;
    public WaterSurface  waterSurface;

    private void Start()
    {
        

        StartCoroutine(InitWhenCharacterExists());
    }

    IEnumerator InitWhenCharacterExists()
    {
        // wait for the local character to spawn
        while (Character.Local == null)
            yield return null;

        detector = Character.Local.GetComponentInChildren<BiomeDetector>();
        if (detector == null)
        {
            Debug.LogError("No BiomeDetector on Player!");
            yield break;
        }

        
        globalVolume     = FindObjectOfType<Volume>();
        directionalLight = GameObject.FindWithTag("Sun")?.GetComponent<Light>();
        waterSurface     = FindObjectOfType<WaterSurface>();

        detector.OnBiomeChanged += ApplyBiomeVisuals;
    }

    void ApplyBiomeVisuals(BiomeData data)
    {
        // 1) Fog & ambient
        RenderSettings.fogColor   = data.fogColor;
        RenderSettings.fogDensity = data.fogDensity;
        RenderSettings.ambientLight = data.ambientColor;

        // 2) Water tweaks
        if (data.waterMaterialOverride != null)
            waterSurface.customMaterial = data.waterMaterialOverride;
        waterSurface.refractionColor = data.waterSurfaceColor;
        waterSurface.scatteringColor = data.waterUnderColor;

        // 3) Post‐process swap
        if (globalVolume != null && data.postProcessProfile != null)
            globalVolume.profile = data.postProcessProfile;

        // 4) Sun tint
        if (directionalLight != null)
            directionalLight.color = data.fogColor * 0.8f;
    }
}
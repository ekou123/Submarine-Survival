using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BiomeVisualManager : MonoBehaviour
{
    public List<BiomeVisualProfile> profiles;
    public Volume globalVolume;
    public Light directionalLight;
    public Renderer oceanRenderer;
    public BiomeDetector detector;

    Dictionary<BiomeType, BiomeVisualProfile> _lookup;


    private void Awake()
    {
        _lookup = new Dictionary<BiomeType, BiomeVisualProfile>();
        foreach (var p in profiles)
        {
            _lookup[p.biomeType] = p;
        }

        detector.OnBiomeChanged += ApplyProfile;
    }

    void ApplyProfile(BiomeType newBiome)
    {
        Debug.Log("Ass");
        if (!_lookup.TryGetValue(newBiome, out var prof))
            return;

        Debug.Log("Omega");

        // 1) Fog and ambient
        RenderSettings.fogColor   = prof.fogColor;
        RenderSettings.fogDensity = prof.fogDensity;
        RenderSettings.ambientLight = prof.ambientColor;

        // 2) Swap or tweak ocean material
        if (prof.waterMaterialOverride != null)
            oceanRenderer.sharedMaterial = prof.waterMaterialOverride;

        var mat = oceanRenderer.sharedMaterial;
        if (mat.HasProperty("_SurfaceColor"))
            mat.SetColor("_SurfaceColor", prof.waterSurfaceColor);
        if (mat.HasProperty("_UnderwaterColor"))
            mat.SetColor("_UnderwaterColor", prof.waterUnderColor);

        // 3) Post-process
        if (globalVolume != null && prof.postProcessProfile != null)
        {
            globalVolume.profile = prof.postProcessProfile;
        }

        // 4) (Optional) sun tint
        if (directionalLight != null)
            directionalLight.color = prof.fogColor * 0.8f;
    }
}

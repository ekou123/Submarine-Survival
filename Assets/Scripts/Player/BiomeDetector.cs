using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDetector : MonoBehaviour
{
    public BiomeType currentBiome;
    public BiomeType defaultBiome = BiomeType.Shallow;

    void OnTriggerEnter(Collider other)
    {
        BiomeVolume biome = other.GetComponent<BiomeVolume>();
        if (biome != null)
        {
            currentBiome = biome.biomeType;
            Debug.Log($"Entered Biome: {currentBiome}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BiomeVolume biome = other.GetComponent<BiomeVolume>();
        if (biome != null && currentBiome == biome.biomeType)
        {
            currentBiome = defaultBiome;
            Debug.Log($"Exited biome, reverting to {currentBiome}");
        }
    }
}

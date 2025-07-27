using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDetector : MonoBehaviour
{
    public BiomeType currentBiome;
    public BiomeType defaultBiome = BiomeType.Test;

    public event System.Action<BiomeType> OnBiomeChanged;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Deez");
        if (other.TryGetComponent<BiomeVolume>(out var vol))
        {

            if (currentBiome != vol.biomeType)
            {

                currentBiome = vol.biomeType;
                OnBiomeChanged?.Invoke(currentBiome);
            }
        }
    }

    public void ChangeBiome(BiomeType biomeType)
    {
         //OnBiomeChanged?.Invoke(biomeType);
    }

    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Nuts");
        if (other.TryGetComponent<BiomeVolume>(out var vol) &&
            currentBiome == vol.biomeType)
        {
            currentBiome = defaultBiome;
            OnBiomeChanged?.Invoke(currentBiome);
        }
    }
}

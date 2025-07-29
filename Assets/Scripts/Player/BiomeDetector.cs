using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDetector : MonoBehaviour
{
    public BiomeData defaultBiomeData;
    public BiomeData currentBiomeData;

    public event System.Action<BiomeData> OnBiomeChanged;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BiomeVolume>(out var vol))
        {
            var newData = vol.biomeData; 
            if (newData != currentBiomeData)
            {
                currentBiomeData = newData;
                OnBiomeChanged?.Invoke(newData);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (other.TryGetComponent<BiomeVolume>(out var vol) && 
        //     vol.biomeData == currentBiomeData)
        // {
        //     currentBiomeData = defaultBiomeData;
        //     OnBiomeChanged?.Invoke(defaultBiomeData);
        // }
    }
}
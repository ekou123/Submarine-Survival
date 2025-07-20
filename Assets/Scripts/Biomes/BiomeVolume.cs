using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BiomeVolume : MonoBehaviour
{
    public BiomeType biomeType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BiomeDetector>(out var det))
        {
            if (det.currentBiome != biomeType)
            {
                Debug.Log("Deez");
                det.currentBiome = biomeType;
                det.ChangeBiome(biomeType);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BiomeDetector>(out var det))
        {
            Debug.Log("Nuts");
            det.currentBiome = det.defaultBiome;
            det.ChangeBiome(det.currentBiome);
        }
    }
    private void Reset()
    {
        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f); // translucent green
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}

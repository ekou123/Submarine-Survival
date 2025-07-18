using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BiomeVolume : MonoBehaviour
{
    public BiomeType biomeType;

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

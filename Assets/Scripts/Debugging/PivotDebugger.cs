using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PivotDebugger : MonoBehaviour
{
    // Draw the “up” axis of the pivot in red so you can see its tilt
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Vector3 up  = transform.up * 2f;   // length 2 units
        Gizmos.DrawLine(pos, pos + up);
        Gizmos.DrawSphere(pos + up, 0.05f);
    }
}
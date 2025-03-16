using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorsOperations
{
    public static Vector3 CalculateDirection (Vector3 target, Vector3 origin)
    {
        Vector3 heading = target - origin;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        return direction;
    }

    public static Vector3 CalculateTarget (Vector3 origin, Vector3 direction)
    {
        return origin + direction;
    }
}

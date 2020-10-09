using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HedgeMath
{
    /// <summary>
    /// Projects a vector along a given normal to split it into two vectors.
    /// </summary>
    /// <param name="Vector">Vector to split</param>
    /// <param name="Normal">Normal to split along</param>
    /// <param name="Planar">Output planar vector</param>
    /// <param name="Vertical">Output vertical vector</param>
    public static void SplitPlanarVector(Vector3 Vector, Vector3 Normal, out Vector3 Planar, out Vector3 Vertical)
    {
        Planar = Vector3.ProjectOnPlane(Vector, Normal);
        Vertical = Vector - Planar;
    }
    /// <summary>
    /// Returns true if two vectors are roughly the same
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="DeadZone">How close can the vectors be to return true</param>
    /// <returns></returns>
    public static bool IsApproximate (Vector3 A, Vector3 B, float DeadZone)
    {
        Vector3 Difference = A - B;
        return Difference.magnitude <= DeadZone;
    }

    public static float ClampFloat (float value, float min, float max)
    {
        float _tmp = value;
        if (_tmp >= max)
            _tmp = max;
        if (_tmp <= min)
            _tmp = min;
        return _tmp;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticTools
{
    public static void SetTag(GameObject go, string tag)
    {
        go.layer = LayerMask.NameToLayer(tag);
        foreach (Transform t in go.transform)
        {
            SetTag(t.gameObject, tag);
        }
    }

    public static string Format(long value)
    {
        if (value > 1000000000000)
        {
            return Mathf.CeilToInt(value / 1000000000f) + "B";
        }
        if (value > 1000000000)
        {
            return Mathf.CeilToInt(value / 1000000f) + "M";
        }
        else if (value>1000000)
        {
            return Mathf.CeilToInt(value / 1000f)+"K";
        }

        return value.ToString();

    }

    public static Vector3[] CurveLine(Vector3 position1, Vector3 position2, float curveCoef = 0.2f)
    {
        int pointsCount = 25;

        List<Vector3> positions = new List<Vector3>();
        
        for (int i = 0; i < pointsCount; i++)
        {
            Vector3 pos = Vector3.Lerp(position1, position2, (float)i/(float)pointsCount);
            pos += Vector3.up * Vector3.Distance(position1, position2)*curveCoef*Mathf.Sin(3.14f*i/(float)pointsCount);
            positions.Add(pos);
        }

        return positions.ToArray();
    }
}
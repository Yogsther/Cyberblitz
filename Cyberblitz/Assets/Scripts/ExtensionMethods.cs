using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    //___object___//

    public static void LogWithOriginTag(this object obj, string message)
    {
        Debug.Log($"[{obj.GetType()}] - {message}");
    }


    //___Position___//

    public static Vector2 ToVector2(this Position position)
    {
        return new Vector2(position.x, position.y);
    }


    //___float__//

    public static float SmoothTo(ref this float value, float target, float delta)
    {
        value = Mathf.Lerp(value, target, delta);

        return value;
    }

    public static float SmoothToAngle(ref this float angle, float target, float delta)
    {
        angle = Mathf.LerpAngle(angle, target, delta);

        return angle;
    }


    //___Vector2___//

    public static Vector2 Round(this Vector2 vector, float roundTo = 1f)
    {
        float x = vector.x != 0f ? Mathf.Round(vector.x / roundTo) * roundTo : 0f;
        float y = vector.y != 0f ? Mathf.Round(vector.y / roundTo) * roundTo : 0f;

        return new Vector2(x, y);
    }

    public static Vector3 ToFlatVector3(this Vector2 vector, float y = 0f)
    {
        return new Vector3(vector.x, y, vector.y);
    }

    public static Vector2Int RoundToVector2Int(this Vector2 vector)
    {
        int x = Mathf.RoundToInt(vector.x);
        int y = Mathf.RoundToInt(vector.y);

        return new Vector2Int(x, y);
    }

    public static Position ToPosition(this Vector2 vector)
    {
        return new Position(vector.x, vector.y);
    }

    //___Vector3___//

    public static Vector2 FlatVector3ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 Flatten(this Vector3 vector, float y = 0)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 ClampToRect(this Vector3 vector, Rect rect, float padding)
    {
        Vector3 clampedVector = new Vector3();

        clampedVector.x = Mathf.Clamp(vector.x, rect.xMin + padding, rect.xMax - padding);
        clampedVector.y = Mathf.Clamp(vector.y, rect.yMin + padding, rect.yMax - padding);

        return vector;
    }

    public static Vector2Int RoundFlatToVector2Int(this Vector3 vector)
    {
        int x = Mathf.RoundToInt(vector.x);
        int y = Mathf.RoundToInt(vector.z);

        return new Vector2Int(x, y);
    }

    public static Vector3Int RoundToVector3Int(this Vector3 vector)
    {
        int x = Mathf.RoundToInt(vector.x);
        int y = Mathf.RoundToInt(vector.y);
        int z = Mathf.RoundToInt(vector.z);

        return new Vector3Int(x, y, z);
    }


    //___Vector2Int___//

    public static Vector2 ToVector2(this Vector2Int vector2Int)
    {
        return vector2Int;
    }

    public static Vector3 ToFlatVector3(this Vector2Int vector, float y = 0f)
    {
        return new Vector3(vector.x, y, vector.y);
    }

    //___Vector3Int___//
    public static Vector2Int FlatToVector2Int(this Vector3Int vector)
    {
        return new Vector2Int(vector.x, vector.z);
    }

    //___Vector2[]___//
    
    public static Vector2[] SetAllIndexesToValue(this Vector2[] v2Array, Vector2 value)
    {
        for(int i = 0; i < v2Array.Length; i++)
        {
            v2Array[i] = value;
        }

        return v2Array;
    }

    //___List<Vector2Int>___//
    public static List<Vector3> ToFlatVector3(this List<Vector2Int> vector2Ints, float y = 0f)
    {
        List<Vector3> vector3s = new List<Vector3>();

        foreach (Vector2Int vector2Int in vector2Ints) vector3s.Add(vector2Int.ToFlatVector3(y));

        return vector3s;
    }


    //___Transform___//

    public static void SetChildLayersRecursive(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = layer;
        }
    }
}
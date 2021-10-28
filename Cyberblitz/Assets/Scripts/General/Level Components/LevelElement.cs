using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class LevelElement
{
    public RectInt gridTransform = new RectInt(Vector2Int.zero, Vector2Int.one);
    public Vector2 gridPositionOrigin => gridTransform.position - Vector2.one * .5f;

    public Vector2 centerOffset => -new Vector2((gridTransform.size.x % 2f) * .5f, (gridTransform.size.y % 2f) * .5f) + Vector2.one * .5f;

    public Vector2 center => (gridTransform.position + centerOffset);

    public GameObject gameObjectPrefab;
    [HideInInspector] public GameObject gameObjectInstance;

    public virtual GameObject Spawn(Transform parent)
    {
        GameObject gameObject = new GameObject(ToString());

        gameObject.transform.parent = parent;
        gameObject.transform.position = center.ToFlatVector3();

        return gameObject;
    }


    public virtual void DrawGizmo()
    {
        Color frameColor = new Color(1f, 1f, 1f);
        Color fillColor = new Color(1f, 1f, 1f, .05f);

        Vector3 worldCenter = center.ToFlatVector3(.01f);
        Vector3 worldSize = gridTransform.size.ToFlatVector3();

        // Fill //

        Gizmos.color = fillColor;

        Gizmos.DrawCube(worldCenter, worldSize);

        // Frame //

        Gizmos.color = frameColor;

        Gizmos.DrawWireCube(worldCenter, worldSize);

    }
}

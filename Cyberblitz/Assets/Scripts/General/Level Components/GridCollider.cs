using System.Collections.Generic;
using UnityEngine;


public enum ColliderType { Full = 4, Half = 2, Quarter = 1 }

[System.Serializable]
public class GridCollider : LevelElement
{
    public ColliderType type = ColliderType.Full;

    private Color fullColor = Color.red;
    private Color halfColor = Color.yellow;
    private Color quarterColor = Color.green;

    public Vector3 cubeOrigin => (gridPositionOrigin).ToFlatVector3();
    public Vector3 cubeSize => gridTransform.size.ToFlatVector3(height);
    public Vector3 cubeCenter => cubeOrigin + cubeSize * .5f;

    public float height => (float)type * .5f;

    public override GameObject Spawn(Transform parent)
    {
        GameObject gameObject = base.Spawn(parent);

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

        gameObject.transform.position = center.ToFlatVector3(height * .5f);
        boxCollider.size = cubeSize;

        gameObject.tag = $"{type} Collider";
        gameObject.layer = 10;

        return gameObject;
    }

    public List<Vector2Int> GetBlockedCoords()
    {
        List<Vector2Int> blockedCoords = new List<Vector2Int>();
 
        for(int x = gridTransform.x; x < gridTransform.x + gridTransform.width; x++)
        {
            for (int y = gridTransform.y; y < gridTransform.y + gridTransform.height; y++)
            {
                blockedCoords.Add(new Vector2Int(x, y));
            }
        }

        return blockedCoords;
    }

    public override void DrawGizmo()
    {
        Color cubeColor = type == ColliderType.Full ? fullColor : type == ColliderType.Half ? halfColor : quarterColor;

        Vector3 worldCenter = center.ToFlatVector3(height * .5f);

        if (!Application.isPlaying)
        {
            Gizmos.color = cubeColor;
            Gizmos.DrawWireCube(worldCenter, cubeSize);
        }

        cubeColor.a = .25f;

        Gizmos.color = cubeColor;
        Gizmos.DrawCube(worldCenter, cubeSize);
    }
}

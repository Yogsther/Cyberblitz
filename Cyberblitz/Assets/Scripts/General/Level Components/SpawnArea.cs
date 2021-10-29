using UnityEditor;
using UnityEngine;

public enum SpawnDirection
{
    Forward,
    Back,
    Left,
    Right
}

[System.Serializable]
public class SpawnArea : LevelElement
{
    public float cameraRotationForTeam;

    public int team;

    public SpawnDirection direction;
    public int testSpawnPoints = 5;
    public Quaternion cameraRotation => Quaternion.AngleAxis(cameraRotationForTeam, Vector3.up);

    public Vector2Int GetSpawnPosition(int i)
    {
        RectInt gt = gridTransform;

        Vector2Int origin = center.RoundToVector2Int() - (gt.size / 2);

        int x = origin.x;
        int y = origin.y;

        bool isVertical = direction == SpawnDirection.Forward || direction == SpawnDirection.Back;
        bool startsFromOrigin = direction == SpawnDirection.Forward || direction == SpawnDirection.Left;

        int offsetX = isVertical ? (i % gt.width) : (i / gt.width);
        int offsetY = isVertical ? (i / gt.height) : (i % gt.height);

        x += startsFromOrigin ? offsetX : gt.width - 1 - offsetX;
        y += startsFromOrigin ? offsetY : gt.height - 1 - offsetY;

        return new Vector2Int(x, y);
    }

    public override void DrawGizmo()
    {
        Gizmos.color = Color.white;

        base.DrawGizmo();

        /*Color gizmoColor = Color.blue;

        Gizmos.color = gizmoColor;
*/
        Vector3 worldCenter = (gridTransform.position + centerOffset).ToFlatVector3();
        Vector3 size = gridTransform.size.ToFlatVector3();

        Vector3 cameraPos = worldCenter + Vector3.up;
        Vector3 cameraFwd = cameraRotation * Vector3.forward;

        Vector2Int spawnFwd = cameraFwd.RoundFlatToVector2Int();

        Gizmos.DrawWireSphere(cameraPos, .25f);
        Gizmos.DrawRay(cameraPos, cameraFwd);

        for (int i = 0; i < testSpawnPoints; i++)
        {
            Vector2Int pos = GetSpawnPosition(i);

            Gizmos.DrawWireSphere(pos.ToFlatVector3(), .4f);
        }

        /*
                Gizmos.DrawWireCube(worldCenter, size);

                gizmoColor.a = .25f;

                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(worldCenter, size);*/

#if UNITY_EDITOR
        Handles.Label(worldCenter, $"Team {team} Spawn");
#endif
        /*
                if (mirror)
                {
                    gizmoColor = Color.red;
                    Gizmos.color = gizmoColor;

                    worldCenter = levelElements.levelGridSize.ToFlatVector3() - (worldCenter + Vector2.one.ToFlatVector3());

                    Gizmos.DrawWireCube(worldCenter, size);

                    gizmoColor.a = .25f;

                    Gizmos.color = gizmoColor;
                    Gizmos.DrawCube(worldCenter, size);

        #if UNITY_EDITOR
                    Handles.Label(worldCenter + Vector3.up, $"Team {team + 1} Spawn");
        #endif
                }
        */

    }
}

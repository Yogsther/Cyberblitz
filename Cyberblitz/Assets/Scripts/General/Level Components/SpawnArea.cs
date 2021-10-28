using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SpawnArea : LevelElement
{
    public float cameraRotationForTeam;

    public int team;

    public Quaternion cameraRotation => Quaternion.AngleAxis(cameraRotationForTeam, Vector3.up);

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

        Gizmos.DrawWireSphere(cameraPos, .25f);
        Gizmos.DrawRay(cameraPos, cameraRotation * Vector3.forward);

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

using System.Collections;
using UnityEngine;

public class GridPathEditor : InGameEditor
{
    private GridPath selectedGridPath;


    private int lastPointIndex => selectedGridPath.points.Count - 1;
    private int nextToLastPointIndex => selectedGridPath.points.Count - 2;

    private Vector2Int lastPoint => selectedGridPath.points[lastPointIndex];
    private Vector2Int nextToLastPoint => selectedGridPath.points[nextToLastPointIndex];


    private Vector3 smoothPos;

    private readonly float smoothRot;


    [ContextMenu("Edit Test Path")]
    public void EditTestPath()
    {
        GridPath myGridPath = new GridPath();

        EditGridPath(ref myGridPath);
    }

    public void EditGridPath(ref GridPath gridPath, float maxLength = -1f)
    {
        StartCoroutine(PathEditing(gridPath, maxLength));
    }

    [ContextMenu("Stop Editing")]
    public void StopEditing()
    {
        selectedGridPath = null;
    }

    private IEnumerator PathEditing(GridPath gridPath, float maxLength = -1f)
    {
        selectedGridPath = gridPath;

        gridPath.OnUpdated += () => OnUpdated?.Invoke();

        yield return null;

        Debug.Log("[GridPathEditor] - Started editing a path");

        while (selectedGridPath == gridPath)
        {

            if (InputManager.TryGetPointerHitLayer(LayerMask.GetMask("Ground"), out RaycastHit groundHit) && InputManager.pointerIsHeld)
            {


                Vector2 mousePoint = groundHit.point.FlatVector3ToVector2();
                Vector2Int point = mousePoint.RoundToVector2Int();

                // gridPath.SetPoints();


                if (gridPath.points.Count > 1)
                {
                    //point = lastPoint + Vector2.ClampMagnitude(point - lastPoint, 1.4f).RoundToVector2Int();



                    if (point == lastPoint)
                    {
                        gridPath.RemovePointAtIndex(lastPointIndex);
                    }

                    if (gridPath.points.Count > 2)
                    {

                        if (Vector2Int.Distance(point, nextToLastPoint) < 2f)
                        {
                            gridPath.RemovePointAtIndex(lastPointIndex);
                        }

                        if (point == nextToLastPoint)
                        {

                            gridPath.RemovePointAtIndex(nextToLastPointIndex);

                            gridPath.RemovePointAtIndex(lastPointIndex);

                        }
                    }


                }

                Debug.Log("Total Length:" + gridPath.GetTotalPathLength());




                gridPath.AddPoint(point);

                //if (maxLength != -1f && gridPath.GetTotalPathLength() > maxLength)
                //{
                //    gridPath.RemovePointAtIndex(gridPath.points.Count - 1);
                //}


            }

            yield return null;
        }

        gridPath.OnUpdated -= () => OnUpdated?.Invoke();

        Debug.Log($"[GridPathEditor] - Stopped editing a path");
    }

    private void OnDrawGizmos()
    {


        if (selectedGridPath != null)
        {
            if (selectedGridPath.points.Count > 0)
            {
                /* foreach (Pathfinder.Node node in Pathfinder.GetPath(selectedGridPath.points[0], selectedGridPath.points[selectedGridPath.points.Count - 1]))
                 {
                     Vector3 dir = node.direction.ToFlatVector3();

                     Gizmos.color = new Color(dir.x, dir.y, dir.z);

                     Gizmos.DrawWireCube(node.position.ToFlatVector3(), Vector3.one);

                     Gizmos.color = Color.white;
                     Gizmos.DrawRay(node.position.ToFlatVector3(), node.direction.ToFlatVector3().normalized);

                 }*/
            }



            selectedGridPath.DrawGizmos();
        }
    }

}

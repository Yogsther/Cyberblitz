using System.Collections;
using UnityEngine;

public class AutoGridPathEditor : InGameEditor
{
    private AutoGridPath selectedGridPath;

    public float lengthCap = -1f;

    /*[ContextMenu("Edit Test Path")]
    public void EditTestPath()
    {
        AutoGridPath myGridPath = new AutoGridPath(Vector2Int.zero);

        EditGridPath(ref myGridPath);
    }*/

    public void EditGridPath(ref AutoGridPath gridPath)
    {
        StartCoroutine(PathEditing(gridPath));
    }

    [ContextMenu("Stop Editing")]
    public void StopEditing()
    {
        selectedGridPath = null;
        lengthCap = -1f;
    }

    private IEnumerator PathEditing(AutoGridPath gridPath)
    {
        selectedGridPath = gridPath;

        yield return null;

        Debug.Log("[GridPathEditor] - Started editing a path");


        int groundLayer = 6;

        while (selectedGridPath == gridPath)
        {


            if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(groundLayer, out RaycastHit groundHit) && InputManager.pointerIsHeld)
            {


                Vector2 mousePoint = groundHit.point.FlatVector3ToVector2();
                Vector2Int point = mousePoint.RoundToVector2Int();

                bool isBlocked = Physics.CheckSphere(point.ToFlatVector3(.5f), .25f);

                if (!isBlocked)
                {
                    Vector2Int originToCurrentTarget = gridPath.target.point - gridPath.origin.point;
                    Vector2Int fromTo = (point - gridPath.origin.point);

                    point = (gridPath.origin.point + Vector2.ClampMagnitude(fromTo, originToCurrentTarget.magnitude + lengthCap)).RoundToVector2Int();

                    gridPath.target.point = point;


                    OnUpdated?.Invoke();

                }
            }

            yield return null;
        }

        Debug.Log($"[GridPathEditor] - Stopped editing a path");
    }

    private void OnDrawGizmos()
    {
        if (selectedGridPath != null)
        {
            selectedGridPath.DrawGizmos();
        }
    }
}

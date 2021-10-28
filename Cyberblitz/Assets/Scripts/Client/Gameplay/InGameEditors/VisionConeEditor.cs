using System.Collections;
using UnityEngine;

public class VisionConeEditor : InGameEditor
{
    private VisionCone selectedVisionCone;



    [ContextMenu("Edit Test Cone")]
    public void EditTestPath()
    {
        VisionCone myVisionCone = new VisionCone();

        myVisionCone.origin = new Vector2Int(5, 5);
        myVisionCone.radius = 5f;
        myVisionCone.angleWidth = 45f;

        EditGridPath(ref myVisionCone);
    }

    public void EditGridPath(ref VisionCone visionCone)
    {
        StartCoroutine(ConeEditing(visionCone));
    }

    public void StopEditing()
    {
        selectedVisionCone = null;
    }

    private IEnumerator ConeEditing(VisionCone visionCone)
    {
        selectedVisionCone = visionCone;

        yield return null;

        Debug.Log("[VisionConeEditor] - Started editing a cone");

        while (selectedVisionCone == visionCone)
        {

            if (InputManager.TryGetPointerHitLayer(6, out RaycastHit groundHit))
            {

                Vector2 mouseHitPoint = groundHit.point.FlatVector3ToVector2();

                Vector2 toMouse = mouseHitPoint - visionCone.origin;

                visionCone.direction = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;


                if (InputManager.pointerIsHeld)
                {
                    OnUpdated?.Invoke();

                    //selectedVisionCone = null;
                }

            }

            yield return null;
        }


        Debug.Log("[VisionConeEditor] - Stopped editing a cone");
    }


    private void OnDrawGizmos()
    {
        if (selectedVisionCone != null) selectedVisionCone.DrawGizmo();
    }

}

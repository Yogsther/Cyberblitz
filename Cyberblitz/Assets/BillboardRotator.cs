using UnityEngine;

public class BillboardRotator : MonoBehaviour
{
    private Camera currentCamera;

    private void Awake()
    {
        currentCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (currentCamera == null || !currentCamera.isActiveAndEnabled)
        {
            currentCamera = Camera.main;
        }

        if (currentCamera != null && currentCamera.isActiveAndEnabled)
        {
            transform.forward = -currentCamera.transform.forward;
        }
    }
}

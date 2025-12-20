using UnityEngine;

public class CameraAspectRatioHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;

    /// <summary>
    /// zooms the width of the camera to fit design
    /// </summary>
    void Start()
    {
        float targetAspect = 16.0f / 9.0f;
        float scaleSize = targetAspect / cam.aspect;
        cam.orthographicSize = cam.orthographicSize * scaleSize;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectRatioLetterBox : MonoBehaviour
{
    // 유지할 목표 비율 (예: 16:9)
    public float targetAspectRatio = 16f / 9f;

    private Camera cam;
    private float lastScreenWidth;
    private float lastScreenHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCameraViewport();
    }

    void Update()
    {
        // 창 크기가 변경되었을 경우에만 갱신
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustCameraViewport();
        }
    }

    void AdjustCameraViewport()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        Rect rect = cam.rect;

        if (scaleHeight < 1.0f)
        {
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
        }

        cam.rect = rect;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}

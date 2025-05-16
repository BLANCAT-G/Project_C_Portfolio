using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CenterZoom : MonoBehaviour
{
    public Camera renderCam;
    public RawImage display;

    [Range(0.5f, 1f)]
    public float zoomRatio = 1f;

    RenderTexture rt;

    void Start()
    {
        rt = new RenderTexture(Screen.width, Screen.height, 16)
        {
            filterMode = FilterMode.Point
        };

        renderCam.targetTexture = rt;

        display.texture = rt;
    }

    void Update()
    {
        // zoomRatio를 0.5 ~ 1.0 범위로 보장
        float z = Mathf.Clamp01(zoomRatio);

        // UV 크롭 계산 (중앙 영역)
        float offset = (1f - z) * 0.5f;
        display.uvRect = new Rect(offset, offset, z, z);
    }

    //void OnDisable()
    //{
    //    if (rt != null)
    //    {
    //        renderCam.targetTexture = null;
    //        rt.Release();
    //        Destroy(rt);
    //    }
    //}

}

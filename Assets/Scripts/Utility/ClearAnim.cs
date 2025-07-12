using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ClearAnim : MonoBehaviour
{
    public ColorType colortype;
    public SpriteRenderer sr;
    public InGameController controller;
    public Camera mainCam;
    public GameObject child;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }
    void LateUpdate()
    {
        if (mainCam == null) 
            return;

        // 뷰포트 (0.5,0.5) = 화면 중앙
        Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 10f);
        // 월드 좌표로 변환
        Vector3 worldPos = mainCam.ViewportToWorldPoint(viewportCenter);
        transform.position = worldPos;
        child.transform.position = worldPos;   
    }
    public void win()
    {

        controller.Win(colortype);
    }
    [ButtonMethod]
    public void TestColor()
    {
        sr.color=colortype.ToColor();
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GridLine : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 originPosition;
    private int width, height;
    private float cellsize = 1;
    private float lineWidth = 0.05f;

    private void Awake()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material.color=Color.black;
    }

    public void DrawGridLine(Vector3 oP, int w, int h, float cs)
    {
        SetGridInfo(oP,w,h,cs);
        if (width < 1 || height < 1) return;
        
        List<Vector3> gridPos = new List<Vector3>();
        
        Vector3 curPos = originPosition;
        gridPos.Add(VCopy(curPos));
        curPos += new Vector3(width * cellsize, 0, 0);
        gridPos.Add(VCopy(curPos));

        int k = 1;
        for (int i = 0; i < height; ++i)
        {
            k *= -1;
            curPos += new Vector3(0, cellsize, 0);
            gridPos.Add(VCopy(curPos));
            curPos += new Vector3(k * width * cellsize, 0, 0);
            gridPos.Add(VCopy(curPos));
        }
        gridPos.Add(VCopy(curPos+new Vector3(lineWidth/2*k,0,0)));

        curPos = originPosition+new Vector3(width * cellsize, height * cellsize, 0);
        gridPos.Add(VCopy(curPos));
        curPos += new Vector3(0, -height * cellsize, 0);
        gridPos.Add(VCopy(curPos));

        k = -1;
        for (int i = 0; i < width; ++i)
        {
            k *= -1;
            curPos += new Vector3(-cellsize, 0, 0);
            gridPos.Add(VCopy(curPos));
            curPos += new Vector3(0, k * height * cellsize, 0);
            gridPos.Add(VCopy(curPos));
        }
        gridPos.Add(VCopy(curPos));

        line.positionCount = gridPos.Count;
        line.SetPositions(gridPos.ToArray());
    }

    public void SetGridInfo(Vector3 oP, int w, int h, float cs)
    {
        originPosition = oP;
        width = w;
        height = h;
        cellsize = cs;
    }

    public Vector3 VCopy(Vector3 v)
    {
        return new Vector3(v.x, v.y, v.x);
    }
}

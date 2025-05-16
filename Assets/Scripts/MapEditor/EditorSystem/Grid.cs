using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Grid<TGridObject>
{
    private int width, height;
    private float cellsize;
    private TGridObject[,] grid;
    private Vector3 originPosition;
    public Grid(int w,int h,float cell, Vector3 originPos)
    {
        this.width = w;
        this.height = h;
        this.originPosition = originPos;
        this.cellsize = cell;
        
        grid = new TGridObject[width,height];
    }

    public int getWidth()
    {
        return this.width;
    }

    public int getHeight()
    {
        return this.height;
    }

    public void setValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            grid[x, y] = value;
        }
    }

    public void setValue(Vector3 worldPos, TGridObject value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        setValue(x, y, value);
    }

    public TGridObject getValue(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return grid[x, y];
        }

        return default(TGridObject);
    }

    public TGridObject getValue(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return getValue(x, y);
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos-originPosition).x / cellsize);
        y = Mathf.FloorToInt((worldPos-originPosition).y / cellsize);
    }
    public Vector3 getPosition(int x, int y)
    {
        return originPosition+new Vector3(x, y) * cellsize;
    }

    public Vector3 getCenterPosition(int x, int y)
    {
        return getPosition(x, y) + new Vector3(cellsize / 2, cellsize / 2, 0);
    }

    public bool inBoundary(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        if (x >= 0 && x < width && y >= 0 && y < height) return true;
        return false;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeButton : MonoBehaviour
{
    public GameObject tiles, decos, walls, objects, filters,grid;
    private int cnt = 5;

    public void OnClickAllVisualizeButton()
    {
        if (cnt == 0)
        {
            SetAll(true);
            cnt = 5;
        }
        else
        {
            SetAll(false);
            cnt = 0;
        }
    }
    
    public void OnClickTileVisualizeButton()
    {
        if (tiles.activeSelf)
        {
            tiles.SetActive(false);
            cnt--;
        }
        else
        {
            tiles.SetActive(true);
            cnt++;
        }
    }
    
    public void OnClickDecoVisualizeButton()
    {
        if (decos.activeSelf)
        {
            decos.SetActive(false);
            cnt--;
        }
        else
        {
            decos.SetActive(true);
            cnt++;
        }
    }
    
    public void OnClickWallVisualizeButton()
    {
        if (walls.activeSelf)
        {
            walls.SetActive(false);
            cnt--;
        }
        else
        {
            walls.SetActive(true);
            cnt++;
        }
    }
    
    public void OnClickObjectVisualizeButton()
    {
        if (objects.activeSelf)
        {
            objects.SetActive(false);
            cnt--;
        }
        else
        {
            objects.SetActive(true);
            cnt++;
        }
    }
    
    public void OnClickFilterVisualizeButton()
    {
        if (filters.activeSelf)
        {
            filters.SetActive(false);
            cnt--;
        }
        else
        {
            filters.SetActive(true);
            cnt++;
        }
    }

    public void OnGridVisualizeButtonclick()
    {
        if(grid.activeSelf) grid.SetActive(false);
        else grid.SetActive(true);
    }

    private void SetAll(bool a)
    {
        tiles.SetActive(a);
        decos.SetActive(a);
        walls.SetActive(a);
        objects.SetActive(a);
        filters.SetActive(a);
    }
    
}

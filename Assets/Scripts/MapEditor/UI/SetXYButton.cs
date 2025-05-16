using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetXYButton : MonoBehaviour
{
    public GameObject setXYView;
    public GameObject mapEditor;
    
    public TMP_InputField inputFieldX;
    public TMP_InputField inputFieldY;

    private void Start()
    {
        mapEditor.SetActive(false);
    }

    public void setMapSize()
    {
        int x = int.Parse(inputFieldX.text);
        int y = int.Parse(inputFieldY.text);
        mapEditor.GetComponent<MapEditor>().setWH(x,y);
        mapEditor.GetComponent<MapEditor>().InitGrid();
        mapEditor.SetActive(true);
        setXYView.SetActive(false);
    }
}
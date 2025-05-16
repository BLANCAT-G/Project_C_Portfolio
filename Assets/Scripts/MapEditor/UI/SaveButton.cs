using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveButton : MonoBehaviour
{
    public GameObject saveView;
    public GameObject mapEditor;
    
    public TMP_InputField inputFieldFileName;
    public TMP_InputField inputFieldMapName;
    // Start is called before the first frame update
    void Start()
    {
        saveView.SetActive(false);
    }

    // Update is called once per frame
    public void Save()
    {
        mapEditor.GetComponent<MapEditor>().Save(inputFieldFileName.text,inputFieldMapName.text);
    }

    public void Close()
    {
        mapEditor.GetComponent<MapEditor>().Resume();
        saveView.SetActive(false);
    }
}

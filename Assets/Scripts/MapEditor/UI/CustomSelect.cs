using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSelect : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Contents;
    public GameObject MapButton;
    public GameObject CreateMapUI;
    public GameObject SetNameUI;
    public GameObject Scroll,Scrollbar;
    public TMP_InputField fileNameField,mapNameField,mapStyleField;
    
    private const string SAVE_EXTENSION = "cfy";
    private static readonly string SAVE_FOLDER = Application.streamingAssetsPath + "/Maps/";
    
    
    private void Awake()
    {
        DirectoryInfo di = new DirectoryInfo(SAVE_FOLDER);
        List<string> fileNames=new List<string>();
        foreach (FileInfo file in di.GetFiles("*.cfy")) fileNames.Add(Path.GetFileNameWithoutExtension(file.Name));
        fileNames.Sort(FileNameCompare);
        
        foreach (string fileName in fileNames)
        {
            GameObject mapButton=Instantiate(MapButton, Contents.transform);
            GridMap.SaveObject saveObject = SaveSystem.LoadObject<GridMap.SaveObject>(fileName);
            mapButton.transform.Find("MapName").GetComponent<TextMeshProUGUI>().text = saveObject.mapName;
            mapButton.GetComponent<Button>().onClick.AddListener((() => OnMapButtonClick(fileName)));
        }
    }

    public void OnMapButtonClick(string fileName)
    {
        CustomManager.Instance.SetFileName(fileName);
        SceneManager.LoadScene("MapEditor");
    }

    public void OnOpenCreateMapUIButtonClick()
    {
        CreateMapUI.SetActive(true);
    }
    
    public void OnCreateMapButtonClick()
    {
        GameObject mapButton=Instantiate(MapButton, Contents.transform);
        GridMap.SaveObject saveObject = new GridMap.SaveObject();
        
        saveObject.width = 10;
        saveObject.height = 10;
        saveObject.mapName = mapNameField.text;
        saveObject.mapStyle = int.Parse(mapStyleField.text);
        mapButton.transform.Find("MapName").GetComponent<TextMeshProUGUI>().text = saveObject.mapName;
        mapButton.GetComponent<Button>().onClick.AddListener((() => OnMapButtonClick(fileNameField.text)));
        SaveSystem.SaveObject(fileNameField.text,saveObject);
        SetNameUI.SetActive(false);
    }
    
    public void OnCloseButtonClick()
    {
        SetNameUI.SetActive(false);
    }
    
    public int FileNameCompare(string a, string b)
    {
        if (a.Length != b.Length) return a.Length < b.Length ? -1 : 1;
        else return a.CompareTo(b);
    }
}

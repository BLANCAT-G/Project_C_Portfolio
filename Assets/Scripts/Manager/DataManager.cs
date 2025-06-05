using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    public string worldName;
    public int curStage;
    public int[] mapProgress;
    public int[] lastMap;
    
    public GameData()
    {
        worldName = "New World";
        curStage = 1;
        mapProgress = new int[3] { 2, 1, 1 };
        lastMap = new int[3] { 1, 1, 1 };
    }
}

public class DataManager : MonoBehaviour
{
    
    private DataManager() { }
    private static DataManager instance;
    public static DataManager Instance => instance;

    public GameData curData = new GameData();
    public int curSlot = -1;
    
    
    private const string SAVE_EXTENSION = "cfy";

    private static readonly string SAVE_FOLDER = Application.streamingAssetsPath + "/Saves/";

    private static bool isInit = false;
    // Start is called before the first frame update
   
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
    }
    
    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public void SaveData()
    {
        Init();
        string data = JsonUtility.ToJson(curData);
        File.WriteAllText(SAVE_FOLDER+"save"+curSlot.ToString()+"."+SAVE_EXTENSION,data);
    }

    public void LoadData()
    {
        Init();
        string data = File.ReadAllText(SAVE_FOLDER + "save" + curSlot.ToString() + "." + SAVE_EXTENSION);
        curData = JsonUtility.FromJson<GameData>(data);
    }
}

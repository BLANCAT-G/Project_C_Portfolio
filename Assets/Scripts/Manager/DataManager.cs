using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
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
        mapProgress = new int[8] { 2, 1, 1, 1, 1, 1, 1, 1 };
        lastMap = new int[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
    }
}

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
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
        KeySettingFunc();
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

    void KeySettingFunc()
    {
        KeySetting.keys[KeyAction.Up] = (KeyCode)PlayerPrefs.GetInt("Key_Up");
        KeySetting.keys[KeyAction.Down] = (KeyCode)PlayerPrefs.GetInt("Key_Down");
        KeySetting.keys[KeyAction.Left] = (KeyCode)PlayerPrefs.GetInt("Key_Left");
        KeySetting.keys[KeyAction.Right] = (KeyCode)PlayerPrefs.GetInt("Key_Right");
        KeySetting.keys[KeyAction.Undo] = (KeyCode)PlayerPrefs.GetInt("Key_Undo");
        KeySetting.keys[KeyAction.ReStart] = (KeyCode)PlayerPrefs.GetInt("Key_Restart");
        for (int i = 0; i < KeySetting.keys.Count; i++)
        {
            if (KeySetting.keys[(KeyAction)i] == 0)
            {
                DefaultSetting();
                break;
            }
        }
    }
    public void DefaultSetting()
    {
        KeySetting.keys.Clear();
        KeyCode[] defaultKeys = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow,
                                                    KeyCode.RightArrow, KeyCode.Z , KeyCode.R };
        for (int i = 0; i < (int)KeyAction.Max; i++)
        {
            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
        }
        PlayerPrefs.SetInt("Key_Up", (int)KeySetting.keys[KeyAction.Up]);
        PlayerPrefs.SetInt("Key_Down", (int)KeySetting.keys[KeyAction.Down]);
        PlayerPrefs.SetInt("Key_Left", (int)KeySetting.keys[KeyAction.Left]);
        PlayerPrefs.SetInt("Key_Right", (int)KeySetting.keys[KeyAction.Right]);
        PlayerPrefs.SetInt("Key_Undo", (int)KeySetting.keys[KeyAction.Undo]);
        PlayerPrefs.SetInt("Key_Restart", (int)KeySetting.keys[KeyAction.ReStart]);
    }
}

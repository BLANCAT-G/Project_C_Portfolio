using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem 
{
    private const string SAVE_EXTENSION = "cfy";

    private static readonly string SAVE_FOLDER = Application.streamingAssetsPath + "/Maps/";

    private static bool isInit = false;
    // Start is called before the first frame update
    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite)
    {
        Init();
        string saveFileName = fileName;
        File.WriteAllText(SAVE_FOLDER+saveFileName+"."+SAVE_EXTENSION,saveString);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite)
    {
        Init();
        string json = JsonUtility.ToJson(saveObject,true);
        Save(fileName, json, overwrite);
    }
    public static void SaveObject(string fileName,object saveObject)
    {
        SaveObject(fileName,saveObject,false);
    }

    public static string Load(string fileName)
    {
        Init();
        if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
            return saveString;
        }

        return null;
    }

    public static TSaveObject LoadObject<TSaveObject>(string fileName)
    {
        Init();
        string saveString = Load(fileName);
        if (saveString != null) return JsonUtility.FromJson<TSaveObject>(saveString);
        return default(TSaveObject);

    }
}

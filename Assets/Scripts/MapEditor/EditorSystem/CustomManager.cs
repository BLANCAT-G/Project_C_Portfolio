using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;

public class CustomManager : MonoBehaviour
{
    private CustomManager(){}
    private static CustomManager instance;
    public static CustomManager Instance => instance;
    
    
    private string fileName;
    [SerializeField] 
    
    private void Awake()
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
    
    public void SetFileName(string f)
    {
        fileName = f;
    }

    public string GetFileName()
    {
        return fileName;
    }
}

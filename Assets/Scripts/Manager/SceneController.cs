using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private SceneController(){}

    private static SceneController instance;
    
    public static SceneController Instance => instance;

    public string sceneID;
    public bool isChange;
    public ColorType colorType;
    //private bool isCustomMap=false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    public void startGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
   
    public void LoadScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = true;
    }
    public void SetSceneID(string s)
    {
        sceneID = s;
    }

}
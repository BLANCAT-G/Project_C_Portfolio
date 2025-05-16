using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InGameController : MonoBehaviour
{
    [SerializeField]
    private SceneTransition transition;
    // Start is called before the first frame update
    void Start()
    {
        SceneController.Instance.isChange = false;
        transition.SetColor(new Color(53f/255f, 53f / 255f, 53f / 255f));
        transition.ReverseExecute();
    }
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.S))
        // {
        //     SceneController.Instance.isChange = true;
        //     int n=Random.Range(0, 10);
        //     transition.cType = (ColorType)n;
        //     transition.SetColor();
        //     transition.Execute();
        // }
    }
    public void Win(ColorType colorType)
    {
        if (GameManager.isEditor) transition.destination = "MapEditor";
        else transition.destination = "StageSelect" + DataManager.Instance.curData.curStage;

        GameData gameData = DataManager.Instance.curData;
        int curstage = gameData.curStage;
        int curmap = gameData.lastMap[curstage-1];
        if (!Convert.ToBoolean(gameData.mapProgress[curstage - 1] & (1 << (curmap+1))))
        {
            gameData.mapProgress[curstage - 1] = gameData.mapProgress[curstage - 1] | (1 << (curmap + 1));
        }

        if (curstage == 1 && curmap == 12 && gameData.mapProgress[curstage] == 1) gameData.mapProgress[curstage] = 2;
        if (curstage == 2 && curmap == 13 && gameData.mapProgress[curstage] == 1) gameData.mapProgress[curstage] = 2;
        DataManager.Instance.SaveData();
        
        SceneController.Instance.isChange = true;
        transition.cType = (ColorType)colorType;
        transition.SetColor();
        transition.Execute();
        SoundBox.instance.PlaySFX("GameClear");
    }
}

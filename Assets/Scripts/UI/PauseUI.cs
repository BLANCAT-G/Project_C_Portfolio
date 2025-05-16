using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject OptionUI;

    public void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            GameManager.Instance.Resume();
        }
    }

    public void OnResumeButtonClick()
    {
        gameObject.SetActive(false);
        GameManager.Instance.Resume();
    }

    public void OnRestartButtonClick()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("SampleScene");
    }

    public void OnOptionButtonClick()
    {
        OptionUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnMapButtonClick()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("StageSelect"+DataManager.Instance.curData.curStage.ToString());
        SoundBox.instance.StopBGM();
        SoundBox.instance.PlayBGM("Title_BGM");

        //string BGM_NAME = "Stage" + MapManager.Instance.fileName[4] + "_BGM";
        //SoundBox.instance.PlayBGM(BGM_NAME);
    }

    public void OnMenuButtonClick()
    {
        Destroy(GameManager.Instance.gameObject);
        DataManager.Instance.curSlot = -1;
        SceneManager.LoadScene("Title");
        SoundBox.instance.PlayBGM("Title_BGM");
    }
}

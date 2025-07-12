using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject OptionUI;
    public void OnResumeButtonClick()
    {
        UIManager.Instance.CloseUI();
        GameManager.Instance.Resume();
    }

    public void OnRestartButtonClick()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("SampleScene");
    }

    public void OnOptionButtonClick()
    {
        UIManager.Instance.CloseUI();
        OptionUI.SetActive(true);
        UIManager.Instance.stackUI.Push(OptionUI);
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

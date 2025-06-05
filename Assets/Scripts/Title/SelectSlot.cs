using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class SelectSlot : MonoBehaviour
{
    public bool[] saveExsist=new bool[3];
    public GameObject[] slots;
    public Sprite[] slotSprites,iconSprites;
    public TitleButton titleButton;
    public GameObject newSlotUI, deleteSlotUI;
    
    private static readonly string SAVE_FOLDER = Application.streamingAssetsPath + "/Saves/";
    
    void Start()
    {
        for(int i=0;i<3;++i) SetSlot(i);
    }

    public void SetSlot(int i)
    {
        if (File.Exists(SAVE_FOLDER + "save" + i.ToString() + ".cfy"))
        {
            saveExsist[i] = true;
            DataManager.Instance.curSlot = i;
            DataManager.Instance.LoadData();
            GameData gameData = DataManager.Instance.curData;
            int stageProgress = 0, mapProgress = 0;
            for(int k=0;k<3;++k) if (gameData.mapProgress[k] > 1) stageProgress = k;
            for(int k=1;k<=20;++k) if (Convert.ToBoolean(gameData.mapProgress[stageProgress] & (1 << k))) mapProgress = k;
            
            slots[i].GetComponent<Image>().sprite = slotSprites[1];
            slots[i].GetComponent<Button>().interactable = true;
            slots[i].transform.Find("Text_Name").gameObject.SetActive(true);
            slots[i].transform.Find("Text_Progress").gameObject.SetActive(true);
            slots[i].transform.Find("StageIcon").gameObject.SetActive(true);
            slots[i].transform.Find("Button_Add").gameObject.SetActive(false);
            slots[i].transform.Find("Button_Trash").gameObject.SetActive(true);

            slots[i].transform.Find("StageIcon").GetComponent<Image>().sprite = iconSprites[stageProgress];
            string s = "stage" + gameData.curStage.ToString();
            slots[i].transform.Find("Text_Name").GetComponent<LocalizeStringEvent>().StringReference.SetReference("Title", s);
            slots[i].transform.Find("Text_Progress").GetComponent<TextMeshProUGUI>().text = (stageProgress+1)+" - "+mapProgress.ToString();
        }
        else
        {
            saveExsist[i] = false;
            slots[i].GetComponent<Image>().sprite = slotSprites[0];
            slots[i].GetComponent<Button>().interactable = false;
            slots[i].transform.Find("Text_Name").gameObject.SetActive(false);
            slots[i].transform.Find("Text_Progress").gameObject.SetActive(false);
            slots[i].transform.Find("StageIcon").gameObject.SetActive(false);
            slots[i].transform.Find("Button_Add").gameObject.SetActive(true);
            slots[i].transform.Find("Button_Trash").gameObject.SetActive(false);
        }
    }

    public void NewSlot(int num)
    {
        GameObject goUI = titleButton.stackUI.Pop();
        goUI.SetActive(false);
        DataManager.Instance.curSlot = num;
        DataManager.Instance.curData = new GameData();
        DataManager.Instance.SaveData();
        SetSlot(num);
        SoundBox.instance.PlaySFX("ButtonClick");
        StartSlot(num);
    }
    
    public void DeleteSlot(int num)
    {
        GameObject goUI = titleButton.stackUI.Pop();
        goUI.SetActive(false);
        if (File.Exists(SAVE_FOLDER  + "save" + num.ToString() + ".cfy"))
            File.Delete(SAVE_FOLDER  + "save" + num.ToString() + ".cfy");
        
        if (File.Exists(SAVE_FOLDER  + "save" + num.ToString() + ".cfy.meta"))
            File.Delete(SAVE_FOLDER  + "save" + num.ToString() + ".cfy.meta");
        
        SetSlot(num);
        SoundBox.instance.PlaySFX("ButtonClick");
    }

    public void CloseUI()
    {
        GameObject goUI = titleButton.stackUI.Pop();
        goUI.SetActive(false);
    }

    public void StartSlot(int num)
    {
        DataManager.Instance.curSlot = num;
        DataManager.Instance.LoadData();
        StartGame();
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    

    public void StartGame()
    {
        SoundBox.instance.PlaySFX("ButtonClick");
        SceneManager.LoadScene("StageSelect");
    }

    public void NewSlotButton(int num)
    {
        newSlotUI.GetComponent<NewSlotUI>().num = num;
        newSlotUI.SetActive(true);
        titleButton.stackUI.Push(newSlotUI);
    }
    
    public void DeleteSlotButton(int num)
    {
        deleteSlotUI.GetComponent<DeleteSlotUI>().num = num;
        deleteSlotUI.SetActive(true);
        titleButton.stackUI.Push(deleteSlotUI);
    }
}

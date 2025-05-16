using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("Resoultion")]
    public static bool Fullscreen=false;
    public static int width;
    public static int height;
    
    [Header("Music")]
    public Slider BGMVolume;
    public Slider SFXVolume;
    public Sprite[] toggleSprites;
    public TextMeshProUGUI resolutionText;
    public Image toggleImage;
    public static float BGMValue=1;
    public static float SFXValue=1;



    [SerializeField]
    private int curResolutionIdx;
    private List<KeyValuePair<int, int>> resolutionList;
    // Start is called before the first frame update
    void Start()
    {
        //데이터 값 로딩

        resolutionList = new List<KeyValuePair<int, int>>
        {
            new KeyValuePair<int, int>(1280, 720),
            new KeyValuePair<int, int>(1600, 900),
            new KeyValuePair<int, int>(1920, 1080),
            new KeyValuePair<int, int>(2560, 1440),
            new KeyValuePair<int, int>(960, 540)
        };
        curResolutionIdx = PlayerPrefs.GetInt("Resolution");
        width = resolutionList[curResolutionIdx].Key;
        height = resolutionList[curResolutionIdx].Value;
        Fullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen", 0));
        resolutionText.text = width.ToString() + " X " + height.ToString();
        SetResolution();
        for (int i=0;i<resolutionList.Count;++i)
        {
            if (width == resolutionList[i].Key && height == resolutionList[i].Value)
            {
                curResolutionIdx = i;
                break;
            }
        }
        toggleImage.sprite = toggleSprites[Fullscreen ? 1 : 0];
        BGMVolume.value = BGMValue;
        SFXVolume.value = SFXValue;
    }
    public void Update()
    {
        BGMValue = BGMVolume.value;
        SFXValue = SFXVolume.value;
        if (GameManager.Instance && Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            GameManager.Instance.Resume();
        }
    }

    public void ToggleFullScreen()
    {
        Fullscreen=!Fullscreen;
        toggleImage.sprite = toggleSprites[Fullscreen ? 1 : 0];
        SetResolution();
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    public void NextResoultionCount()
    {
        curResolutionIdx++;
        if(curResolutionIdx>= resolutionList.Count)
        {
            curResolutionIdx%= resolutionList.Count;
        }
        width = resolutionList[curResolutionIdx].Key;
        height = resolutionList[curResolutionIdx].Value;
        resolutionText.text = width.ToString() + " X " + height.ToString();
        SetResolution();
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    public void PrevResolutionCount()
    {
        curResolutionIdx--;
        if (curResolutionIdx <= -1)
        {
            curResolutionIdx = resolutionList.Count-1;
        }
        width = resolutionList[curResolutionIdx].Key;
        height = resolutionList[curResolutionIdx].Value;
        resolutionText.text = width.ToString() + " X " + height.ToString();
        SetResolution();
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    public void LanguageButtonClick()
    {
        LocalManager.Instance.ChangeLocale();
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    public void SetResolution()
    {
        Screen.SetResolution(width, height, Fullscreen);
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
        if (GameManager.Instance)
        {
            GameManager.Instance.Resume();
        }
        SaveOption();
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    public void SaveOption()
    {
        PlayerPrefs.SetInt("Language",LocalManager.Instance.curLocale);
        PlayerPrefs.SetInt("FullScreen", Convert.ToInt32(Fullscreen));
        PlayerPrefs.SetInt("Resolution", curResolutionIdx);
        PlayerPrefs.SetFloat("BGMvalue", BGMValue);
        PlayerPrefs.SetFloat("SFXvalue", SFXValue);

    }
}

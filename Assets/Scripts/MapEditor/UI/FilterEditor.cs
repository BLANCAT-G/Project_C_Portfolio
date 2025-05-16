using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FilterEditor : MonoBehaviour
{
    private GameObject filterEditor;
    private FilterInfo filterInfo;
    private FilterSegment[] filterSegments;
    private Button[] curColorButton;
    private Button[,] colorButtons;
    private bool isSetting;

    
    public MapEditor mapEditor;
    public GameObject colorButton;
    public GameObject[] filterPanels;
    public GameObject[] filters;
    public Toggle[] existToggle, oneTimeToggle,alphaToggle;
    public TMP_Dropdown[] dropdownType;
    public void Awake()
    {
        filterEditor = this.gameObject;
        curColorButton = new Button[2];
        colorButtons = new Button[2, 12];
        filterSegments = new FilterSegment[2];
        SetColorButton();
        gameObject.SetActive(false);
        filterPanels[1].SetActive(false);
    }
    
    public void SetColorButton()
    {
        for (int i = 0; i < 2; ++i)
        {
            int filternum = i;
            for (int j = 0; j < 12; ++j)
            {
                int index = j;
                GameObject b = Instantiate(colorButton,filterPanels[i].transform);
                colorButtons[i, j] = b.GetComponent<Button>();
                b.transform.Find("Image").GetComponent<Image>().color=((ColorType)index).ToColor();
                b.GetComponent<RectTransform>().anchoredPosition=new Vector2(40+35*(j%6),-60-35*(j/6));
                b.GetComponent<Button>().onClick.AddListener(() => OnClickSetColor(filternum,index));
            }
        }
        
    }
    
    public void SetObj(FilterInfo f)
    {
        filterInfo = f;
        filterSegments = new FilterSegment[2] { filterInfo.filter1, filterInfo.filter2 };
        isSetting = true;
        for (int i = 0; i < 2; ++i)
        {
            existToggle[i].isOn = filterSegments[i].filterType == FilterType.Null ? false : true;
            if (existToggle[i].isOn)
            {
                filters[i].SetActive(true);
                SetFilterColor(filters[i],filterSegments[i].colorType.ToColor());
                SetCurColorButton(i,colorButtons[i,(int)(filterSegments[i].colorType)]);
                oneTimeToggle[i].interactable = true;
                dropdownType[i].interactable = true;
                alphaToggle[i].interactable = true;
                oneTimeToggle[i].isOn = filterSegments[i].isOneTime;
                dropdownType[i].value = (int)filterSegments[i].filterType - 1;
                alphaToggle[i].isOn = filterSegments[i].isAlpha;
            }
            else
            {
                filters[i].SetActive(false);
                oneTimeToggle[i].interactable = false;
                dropdownType[i].interactable = false;
                alphaToggle[i].interactable = false;
                oneTimeToggle[i].isOn = false;
                dropdownType[i].value = 0;
                alphaToggle[i].isOn = false;
            }
            
        }

        isSetting = false;
    }

    public void SetFilterColor(GameObject f,Color c)
    {
        Transform tmp = f.transform;
        tmp.Find("ArrowBase").GetComponent<Image>().color = c;
        tmp.Find("FrontBase").GetComponent<Image>().color = c;
        tmp.Find("BackBase").GetComponent<Image>().color = c;
    }
    
    public void SetCurColorButton(int filternum, Button b)
    {
        if(curColorButton[filternum]) curColorButton[filternum].transform.Find("Outline").gameObject.SetActive(false);
        curColorButton[filternum] = b;
        curColorButton[filternum].transform.Find("Outline").gameObject.SetActive(true);
    }
    
    public void OnClickSetColor(int filternum,int i)
    {
        SetCurColorButton(filternum,EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        filterSegments[filternum].colorType= (ColorType)i;
        SetFilterColor(filters[filternum],((ColorType)i).ToColor());
        mapEditor.NeedUpdate();
    }
    
    public void OnClickCloseButton()
    {
        filterEditor.SetActive(false);
        mapEditor.AddSaveObject();
        mapEditor.NeedUpdate();
        mapEditor.Resume();
    }

    public void OnChangeFilterButton(int i)
    {
        filterPanels[i].SetActive(true);
        filterPanels[1-i].SetActive(false);
    }

    public void OnExistToggleClick(int i)
    {
        if (isSetting) return;
        if (existToggle[i].isOn)
        {
            filters[i].SetActive(true);
            filterSegments[i].filterType = FilterType.None;
            filterSegments[i].isOneTime = false;
            oneTimeToggle[i].isOn = false;
            oneTimeToggle[i].interactable = true;
            dropdownType[i].value = 0;
            dropdownType[i].interactable = true;
            alphaToggle[i].isOn = false;
            alphaToggle[i].interactable = true;
        }
        else
        {
            if (!existToggle[1 - i].isOn)
            {
                existToggle[i].isOn = true;
                return;
            }
            filters[i].SetActive(false);
            filterSegments[i].filterType = FilterType.Null;
            filterSegments[i].isOneTime = false;
            oneTimeToggle[i].isOn = false;
            oneTimeToggle[i].interactable = false;
            dropdownType[i].value = 0;
            dropdownType[i].interactable = false;
            alphaToggle[i].isOn = false;
            alphaToggle[i].interactable = false;
        }
        mapEditor.NeedUpdate();
    }

    public void OnOneTimeToggleClick(int i)
    {
        if (isSetting) return;
        filterSegments[i].isOneTime = oneTimeToggle[i].isOn;
        mapEditor.NeedUpdate();
    }

    public void OnAlphaToggleClick(int i)
    {
        if (isSetting) return;
        filterSegments[i].isAlpha = alphaToggle[i].isOn;
        mapEditor.NeedUpdate();
    }

    public void OnDropdownTypeChanged(int i)
    {
        if (isSetting) return;
        if (dropdownType[i].value == 0) filterSegments[i].filterType = FilterType.None;
        else if (dropdownType[i].value == 1) filterSegments[i].filterType = FilterType.Mix;
        else if (dropdownType[i].value == 2) filterSegments[i].filterType = FilterType.Substract;
        mapEditor.NeedUpdate();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjEditor : MonoBehaviour
{
    private GameObject objEditor;
    private ObjInfo objInfo;
    private Button curColorButton;
    private Button[] colorButtons;
    private Image image, baseImage;
    private Canvas imageCanvas;
    
    public TextMeshProUGUI sandCount;
    public Toggle alphaToggle;
    public MapEditor mapEditor;
    public GameObject colorButton;

    public void Awake()
    {
        objEditor = this.gameObject;
        colorButtons = new Button[12];
        image = objEditor.transform.Find("Image").GetComponent<Image>();
        baseImage = objEditor.transform.Find("Base").GetComponent<Image>();
        imageCanvas = objEditor.transform.Find("Image").GetComponent<Canvas>();
        SetColorButton();
        gameObject.SetActive(false);
    }

    public void SetColorButton()
    {
        for (int i = 0; i < 12; ++i)
        {
            int index = i;
            GameObject b = Instantiate(colorButton,this.transform);
            colorButtons[i] = b.GetComponent<Button>();
            b.transform.Find("Image").GetComponent<Image>().color=((ColorType)index).ToColor();
            b.GetComponent<RectTransform>().anchoredPosition=new Vector2(40+35*(i%6),-200-35*(i/6));
            b.GetComponent<Button>().onClick.AddListener(() => OnClickSetColor(index));
        }
    }
    
    public void OnClickSetColor(int i)
    {
        SetCurColorButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        objInfo.colorType = (ColorType)i;
        if (objInfo.objType == ObjType.Stamp) baseImage.sprite = mapEditor.stampSpritesArr[i];
        else image.color = ((ColorType)i).ToColor();
        mapEditor.NeedUpdate();
    }
    
    public void SetCurColorButton(Button button)
    {
        if(curColorButton) curColorButton.transform.Find("Outline").gameObject.SetActive(false);
        curColorButton = button;
        curColorButton.transform.Find("Outline").gameObject.SetActive(true);
    }

    public void SetObj(ObjInfo o)
    {
        objInfo = o;
        image.sprite = mapEditor.objSpritesArr[(int)(o.objType)];
        baseImage.sprite = mapEditor.objBaseSpritesArr[(int)(o.objType)];
        alphaToggle.isOn = o.isAlpha;
        
        if (o.objType == ObjType.Stamp) image.color=Color.clear;
        else image.color = o.colorType.ToColor();
        
        if (o.objType == ObjType.Paint || o.objType == ObjType.Brush) imageCanvas.sortingOrder = 3;
        else imageCanvas.sortingOrder = 1;

        if (o.objType==ObjType.SandColor) sandCount.text = o.sandCount.ToString();
        SetCurColorButton(colorButtons[(int)o.colorType]);
    }

    public void OnClickCloseButton()
    {
        objEditor.SetActive(false);
        mapEditor.AddSaveObject();
        mapEditor.NeedUpdate();
        mapEditor.Resume();
    }
    
    public void OnClickCountUpButton()
    {
        if (objInfo.sandCount < 15) objInfo.sandCount++;
        sandCount.text = objInfo.sandCount.ToString();
        mapEditor.NeedUpdate();
    }

    public void OnClickCountDownButton()
    {
        if (objInfo.sandCount > 1) objInfo.sandCount--;
        sandCount.text = objInfo.sandCount.ToString();
        mapEditor.NeedUpdate();
    }

    public void OnAlphaToggleClick()
    {
        objInfo.isAlpha = alphaToggle.isOn;
        mapEditor.NeedUpdate();
    }
}

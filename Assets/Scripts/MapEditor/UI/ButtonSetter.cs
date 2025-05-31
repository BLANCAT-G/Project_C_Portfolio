using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ButtonSetter : MonoBehaviour
{
    public GameObject imageButton,colorButton;
    public GameObject[] wallPage;
    public GameObject tilePage, decoPage, fwallPage, objPage, filterPage,colorPage;
    public Button curButton,curColorButton;
    public MapEditor mapEditor;
    private Image image, baseimage;
    private Sprite[] spritesArr, baseSpritesArr, noneColorSpritesArr;

    public void setButton()
    {
        
        spritesArr = mapEditor.decoSpritesArr;
        for (int i = 1; i < spritesArr.Length; ++i)
        {
            int index = i;
            GameObject b = Instantiate(imageButton,decoPage.transform);
            image=b.transform.Find("Image").GetComponent<Image>();
            image.sprite = spritesArr[i];
            image.color=Color.white;
            b.GetComponent<RectTransform>().anchoredPosition=new Vector2(41+61*((i-1)%3),-41-61*((i-1)/3));
            b.GetComponent<Button>().onClick.AddListener(() => OnClickSetDeco(index));
        }
            
        spritesArr = mapEditor.wallSpritesArr;
        int pagenum = spritesArr.Length / 21 + (spritesArr.Length % 21 == 0 ? 0 : 1);
        for (int i = 0; i < pagenum; ++i)
        {
            for (int j = 0; j < 21; ++j)
            {
                if (i * 21 + j + 1== spritesArr.Length) break;
                int index = i * 21 + j + 1;
                int k = i;
                GameObject b = Instantiate(imageButton,wallPage[i].transform);
                image=b.transform.Find("Image").GetComponent<Image>();
                image.sprite = spritesArr[index];
                image.color=Color.white;
                b.GetComponent<RectTransform>().anchoredPosition=new Vector2(41+61*((index-1)%21%3),-41-61*((index-1)%21/3));
                b.GetComponent<Button>().onClick.AddListener(() => OnClickSetWall(index));
                wallPage[i].transform.Find("Button_Left").GetComponent<Button>().onClick.AddListener((() =>
                    OnSlideButtonClick( wallPage[k], wallPage[(k + pagenum - 1) % pagenum],2)));
                wallPage[i].transform.Find("Button_Right").GetComponent<Button>().onClick.AddListener((() =>
                    OnSlideButtonClick( wallPage[k], wallPage[(k + 1) % pagenum],2)));
            }
        }

        spritesArr = mapEditor.fwallSpritesArr;
        for (int i = 1; i < spritesArr.Length; ++i)
        {
            int index = i;
            GameObject b = Instantiate(imageButton,fwallPage.transform);
            image=b.transform.Find("Image").GetComponent<Image>();
            image.sprite = spritesArr[i];
            image.color=Color.white;
            b.GetComponent<RectTransform>().anchoredPosition=new Vector2(41+61*((i-1)%3),-41-61*((i-1)/3));
            b.GetComponent<Button>().onClick.AddListener(() => OnClickSetFWall(index));
        }
        
        
        spritesArr = mapEditor.objSpritesArr;
        baseSpritesArr = mapEditor.objBaseSpritesArr;
        noneColorSpritesArr = mapEditor.noneColorObjSpritesArr;
        for (int i = 1; i < spritesArr.Length; ++i)
        {
            int index = i;
            GameObject b = Instantiate(imageButton,objPage.transform);
            b.GetComponent<RectTransform>().anchoredPosition=new Vector2(41+61*((i-1)%3),-41-61*((i-1)/3));
            b.GetComponent<Button>().onClick.AddListener(() => OnClickSetObject(index));
            if (noneColorSpritesArr[i])
            {
                baseimage = b.transform.Find("Base").GetComponent<Image>();
                baseimage.sprite = noneColorSpritesArr[i];
                baseimage.color=Color.white;
                continue;
            }
            
            if (spritesArr[i])
            {
                image=b.transform.Find("Image").GetComponent<Image>();
                image.sprite = spritesArr[i];
                image.color = Color.red;
                if ((ObjType)i == ObjType.Paint) b.transform.Find("Image").GetComponent<Canvas>().sortingOrder = 3;
            }
            
            baseimage=b.transform.Find("Base").GetComponent<Image>();
            baseimage.sprite = baseSpritesArr[i];
            baseimage.color=Color.white;
        }

        for (int i = 0; i < 12; ++i)
        {
            int index = i;
            GameObject b = Instantiate(colorButton,colorPage.transform);
            image=b.transform.Find("Image").GetComponent<Image>();
            image.color = ((ColorType)index).ToColor();
            b.GetComponent<RectTransform>().anchoredPosition=new Vector2(20+35*(i%5),-20-35*(i/5));
            b.GetComponent<Button>().onClick.AddListener(() => OnClickSetColor(index));
        }
    }
    

    public void OnClickSetDeco(int i)
    {
        SetCurButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        mapEditor.setDecoValue(new DecoInfo(i));
        mapEditor.paletteMode = MapEditor.PaletteMode.Deco;
    }
    
    public void OnClickSetWall(int i)
    {
        SetCurButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        mapEditor.setWallValue(new WallInfo(i));
        mapEditor.paletteMode = MapEditor.PaletteMode.Wall;
    }
    
    public void OnClickSetFWall(int i)
    {
        SetCurButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        mapEditor.setFWallValue(new FWallInfo(i));
        mapEditor.paletteMode = MapEditor.PaletteMode.FilterWall;
    }
    
    public void OnClickSetObject(int i)
    {
        SetCurButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        if(((ObjType)i)==ObjType.SandColor) mapEditor.setObjectValue(new ObjInfo((ObjType)i,mapEditor.getColorValue(),1));
        else mapEditor.setObjectValue(new ObjInfo((ObjType)i,mapEditor.getColorValue()));
        mapEditor.paletteMode = MapEditor.PaletteMode.Object;
    }

    public void OnClickSetFilter()
    {
        FilterSegment tmpsegment = new FilterSegment(FilterType.None, mapEditor.getColorValue());
        SetCurButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        mapEditor.setFilterValue(new FilterInfo(tmpsegment.DeepCopy(),tmpsegment.DeepCopy()));
        mapEditor.paletteMode = MapEditor.PaletteMode.Filter;
    }

    public void OnClickSetLock()
    {
        SetCurButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        mapEditor.paletteMode = MapEditor.PaletteMode.Lock;
    }

    public void OnClickSetColor(int i)
    {
        SetCurColorButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        mapEditor.setColorValue((ColorType)i);
    }

    public void SetCurButton(Button button)
    {
        if(curButton) curButton.transform.Find("Outline").gameObject.SetActive(false);
        curButton = button;
        curButton.transform.Find("Outline").gameObject.SetActive(true);
    }

    public void OnSlideButtonClick(GameObject a, GameObject b,int type)
    {
        a.SetActive(false);
        b.SetActive(true);
        if (type == 2) ChangeWindow.Instance.WallPanel = b;
    }

    public void SetCurColorButton(Button button)
    {
        if(curColorButton) curColorButton.transform.Find("Outline").gameObject.SetActive(false);
        curColorButton = button;
        curColorButton.transform.Find("Outline").gameObject.SetActive(true);
    }
}
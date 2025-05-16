using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockEditor : MonoBehaviour
{
    private GameObject lockEditor;
    private LockInfo lockInfo;
    private Button[] curColorButton;
    private Button[,] colorButtons;
    private Image[] keys;
    private Canvas imageCanvas;
    
    public MapEditor mapEditor;
    public GameObject colorButton;
    
    public void Awake()
    {
        lockEditor = this.gameObject;
        keys = new Image[3];
        curColorButton = new Button[3];
        colorButtons = new Button[3, 12];
        for (int i = 0; i < 3; ++i)
        {
            keys[i] = lockEditor.transform.Find("Key" + i.ToString()).GetComponent<Image>();
        }
        SetColorButton();
        gameObject.SetActive(false);
    }
    
    public void SetColorButton()
    {
        for (int i = 0; i < 3; ++i)
        {
            int keynum = i;
            for (int j = 0; j < 12; ++j)
            {
                int index = j;
                GameObject b = Instantiate(colorButton,this.transform);
                colorButtons[i, j] = b.GetComponent<Button>();
                b.transform.Find("Image").GetComponent<Image>().color=((ColorType)index).ToColor();
                b.GetComponent<RectTransform>().anchoredPosition=new Vector2(40+35*(j%6),-200-100*i-35*(j/6));
                b.GetComponent<Button>().onClick.AddListener(() => OnClickSetColor(keynum,index));
            }
        }
        
    }
    
    public void SetObj(LockInfo l)
    {
        lockInfo = l;
        for (int i = 0; i < 3; ++i)
        {
            keys[i].color = l.KeyArr[i].ToColor();
            SetCurColorButton(i,colorButtons[i,(int)(l.KeyArr[i])]);
        }
    }

    public void SetCurColorButton(int keynum, Button b)
    {
        if(curColorButton[keynum]) curColorButton[keynum].transform.Find("Outline").gameObject.SetActive(false);
        curColorButton[keynum] = b;
        curColorButton[keynum].transform.Find("Outline").gameObject.SetActive(true);
    }
    
    public void OnClickSetColor(int keynum,int i)
    {
        SetCurColorButton(keynum,EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
        lockInfo.KeyArr[keynum] = (ColorType)i;
        keys[keynum].color=((ColorType)i).ToColor();
        mapEditor.NeedUpdate();
    }
    
    public void OnClickCloseButton()
    {
        lockEditor.SetActive(false);
        mapEditor.AddSaveObject();
        mapEditor.NeedUpdate();
        mapEditor.Resume();
    }
}

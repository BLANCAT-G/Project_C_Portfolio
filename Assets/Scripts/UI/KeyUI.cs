using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum KeyAction
{
    Up,
    Down, 
    Left, 
    Right,
    Undo,
    ReStart,
    Max,
}


public class KeyUI : MonoBehaviour
{
    [SerializeField]
    private bool onChanged;
    private KeyCode[] defaultKeys = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow,
                                                    KeyCode.RightArrow, KeyCode.Z , KeyCode.R };

    [SerializeField]
    private TextMeshProUGUI[] keyText;

    public GameObject noticeText;
    public int currentKey;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    public void Init()
    {
        currentKey = -1;
        for(int i=0;i<KeySetting.keys.Count;i++)
        {
            if(KeySetting.keys[(KeyAction)i]==0)
            {
                DefaultSetting();
                break;
            }    
        }
        for(int i=0;i<keyText.Length;i++)
        {
            keyText[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }
    public void SaveOption()
    {
        noticeText.SetActive(false);
        onChanged = false;
        currentKey = -1;
        PlayerPrefs.SetInt("Key_Up", (int)KeySetting.keys[KeyAction.Up]);
        PlayerPrefs.SetInt("Key_Down", (int)KeySetting.keys[KeyAction.Down]);
        PlayerPrefs.SetInt("Key_Left", (int)KeySetting.keys[KeyAction.Left]);
        PlayerPrefs.SetInt("Key_Right", (int)KeySetting.keys[KeyAction.Right]);
        PlayerPrefs.SetInt("Key_Undo", (int)KeySetting.keys[KeyAction.Undo]);
        PlayerPrefs.SetInt("Key_Restart", (int)KeySetting.keys[KeyAction.ReStart]);
    }
    public void DefaultSetting()
    {
        noticeText.SetActive(false);
        onChanged = false;
        currentKey = -1;
        KeySetting.keys.Clear();
        for(int i=0;i<(int)KeyAction.Max;i++)
        {
            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
            keyText[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
        SaveOption();
    }

    public void OnGUI()
    {
        if (currentKey == -1)
            return;
        Event keyEvent = Event.current;
        if(keyEvent.isKey&&onChanged)
        {
            if (keyEvent.keyCode == KeyCode.Escape)
                return;
            if (keyEvent.keyCode == KeyCode.None)
                return;
            if (isDuplicate(currentKey, keyEvent.keyCode))
            {
                return;
            }
            KeySetting.keys[(KeyAction)currentKey] = keyEvent.keyCode;
            keyText[currentKey].text = KeySetting.keys[(KeyAction)currentKey].ToString();
            currentKey = -1;
            onChanged = false;
            noticeText.SetActive(false);
        }
    }

    public void OnDisable()
    {
        noticeText.SetActive(false);
    }
    public void ChangeKey(int num)
    {
        currentKey = num;
        onChanged = true;
        noticeText.SetActive(true);
        noticeText.GetComponent<TextMeshProUGUI>().text = "키를 입력해주세요.";
    }
    public void OnCloseButtonClick()
    {
        noticeText.SetActive(false);
        onChanged = false;
        currentKey = -1;

        SoundBox.instance.PlaySFX("ButtonClick");
        UIManager.Instance.CloseUI();
    }
    public bool isDuplicate(int idx, KeyCode key)
    {
        for (int i = 0; i < (int)KeyAction.Max; i++)
        {
            if (i == idx)
                continue;
            if (KeySetting.keys[(KeyAction)i] == key)
            {
                noticeText.GetComponent<TextMeshProUGUI>().text = "이미 사용 중인 키입니다.";
                return true;
            }
        }
        return false;
    }
}

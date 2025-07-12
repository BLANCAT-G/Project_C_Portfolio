using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private UIManager() { }

    private static UIManager instance;
    public static UIManager Instance => instance;
    
    public Stack<GameObject> stackUI=new Stack<GameObject>();
    public GameObject blackBack;
    public GameObject RestartPanel,PausePanel;
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (GameManager.isEditor) return;
        if(stackUI.Count==0)
            blackBack.SetActive(false);
        else
            blackBack.SetActive(true);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (GameManager.Instance && stackUI.Count == 0)
            {
                PausePanel.SetActive(true);
                stackUI.Push(PausePanel);
                GameManager.Instance.Pause();
            }
            else
            {
                 CloseUI();
            }
           
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.ReStart]))
        {
            if (GameManager.Instance && stackUI.Count == 0)
            {
                RestartPanel.SetActive(true);
                stackUI.Push(RestartPanel);
                GameManager.Instance.Pause();
            }
        }
    }
    
    public void CloseUI()
    {
        if (stackUI.Count > 0)
        {
            GameObject goUI = stackUI.Pop();
            goUI.SetActive(false);
            SoundBox.instance.PlaySFX("ButtonClick");

            if (goUI.name.Equals("Panel_Option"))
            {
                goUI.GetComponent<OptionUI>().SaveOption();
            }
            else if (goUI.name.Equals("Panel_Pause") || goUI.name.Equals("Panel_Restart"))
            {
                GameManager.Instance.Resume();
            }
        }
    }

    public void CloseAllUI()
    {
        while (stackUI.Count > 0)
        {
            GameObject goUI = stackUI.Pop();
            goUI.SetActive(false);
        }
    }
    
}

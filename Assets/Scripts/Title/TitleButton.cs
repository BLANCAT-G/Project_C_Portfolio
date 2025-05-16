using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public GameObject SaveUI, OptionUI;
    public Stack<GameObject> stackUI=new Stack<GameObject>();
    // Start is called before the first frame update
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(stackUI.Count>0)
            {
                GameObject goUI = stackUI.Pop();
                goUI.SetActive(false);
                if(goUI.name.Equals("Panel_Option"))
                {
                    goUI.GetComponent<OptionUI>().SaveOption();
                }
            }
            
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.isEditor = true;
            SceneManager.LoadScene("CustomSelect");
        }
    }
    public void PlayButton()
    {
        GameManager.isEditor = false;
        SaveUI.SetActive(true);
        stackUI.Push(SaveUI);
        SoundBox.instance.PlaySFX("ButtonClick");
    }

    public void OnEditorButtonClick()
    {
        GridMap.SaveObject saveObject = new GridMap.SaveObject();
        
        saveObject.width = 10;
        saveObject.height = 10;
        saveObject.mapName = "Test";
        saveObject.mapStyle = 0;
        SaveSystem.SaveObject("test",saveObject);
        CustomManager.Instance.SetFileName("test");
        GameManager.isEditor = true;
        SceneManager.LoadScene("MapEditor");
        
        SoundBox.instance.PlaySFX("ButtonClick");
    }

    public void OnOptionButtonClick()
    {
        OptionUI.SetActive(true);
        stackUI.Push(OptionUI);
        SoundBox.instance.PlaySFX("ButtonClick");
    }
    public void OnLocaleButtonClick()
    {
        LocalManager.Instance.ChangeLocale();
        SoundBox.instance.PlaySFX("ButtonClick");
        PlayerPrefs.SetInt("Language", LocalManager.Instance.curLocale);
    }
    public void OnExitButtonClick()
    {
        SoundBox.instance.PlaySFX("ButtonClick");
        Application.Quit();
    }
}

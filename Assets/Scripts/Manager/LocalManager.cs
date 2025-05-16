using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Localization.Settings;

public class LocalManager : MonoBehaviour
{
    private LocalManager() { }

    private static LocalManager instance;

    public static LocalManager Instance => instance;
    bool isChanging;
    public int curLocale = 2;
    private int max = 3;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
        curLocale = PlayerPrefs.GetInt("Language");
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[curLocale];
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            ChangeLocale();
        }
    }
    public void ChangeLocale()
    {
        if (isChanging)
        {
            return;
        }
        curLocale++;
        int idx = curLocale % max;
        StartCoroutine(ChangeRoutine(idx));
        curLocale = idx;
    }

    IEnumerator ChangeRoutine(int _index)
    {
        isChanging = true;

        yield return LocalizationSettings.InitializationOperation; //초기화진행
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_index];
        isChanging = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Components;
public class StageMgr : MonoBehaviour
{
    private StageMgr() { }

    private static StageMgr instance;

    public static StageMgr Instance => instance;

    public int currentStage = 0;
    public StageNode[] stageNodes;
    public StageNode curNode;

    [SerializeField]
    private LocalizeStringEvent LocalStageName;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        for (int i = 0; i < 3; ++i)
        {
            stageNodes[i].isLock = DataManager.Instance.curData.mapProgress[i] > 1 ? false : true;
        }
    }

    void Update()
    {
        if (curNode != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneChange();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToNext();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToPrev();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DataManager.Instance.curSlot = -1;
            SceneManager.LoadScene("Title");
        }
    }

    public void SceneChange()
    {
        string sceneName = "StageSelect" + currentStage.ToString();
        DataManager.Instance.curData.curStage = currentStage;
        DataManager.Instance.SaveData();
        //차후에 SceneController로 대체
        SceneManager.LoadScene(sceneName);
    }
    public void SceneChange(int id)
    {
        string sceneName = "StageSelect" + id.ToString();
        DataManager.Instance.curData.curStage = id;
        DataManager.Instance.SaveData();
        //차후에 SceneController로 대체
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void FocusChange(StageNode node)
    {
        currentStage = node.GetID();
        if(curNode!=null)
            curNode.MouseExitEvent();
        curNode = node;
        curNode.MouseEnterEvent();
        LocalStageName.StringReference.SetReference("Title", "stage"+ currentStage.ToString());
    }
    public void MoveToNext()
    {
        if (curNode.nextNode == null)
            return;
        if (curNode.nextNode.isLock == true)
            return;
        FocusChange(curNode.nextNode as StageNode);     
    }
    public void MoveToPrev()
    {
        if (curNode.prevNode == null)
            return;
        FocusChange(curNode.prevNode as StageNode);
    }
}

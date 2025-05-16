using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class MapSelectMgr : MonoBehaviour
{
    // Start is called before the first frame update
    private MapSelectMgr() {}
    private static MapSelectMgr instance;
    public static MapSelectMgr Instance => instance;
    [SerializeField]
    private string sceneID;
    public SceneTransition transition;
    public GameObject player;

    private bool isMoving = false;
    private WaitForSeconds ws = new WaitForSeconds(0.012f);
    private WaitForSeconds ws5 = new WaitForSeconds(5f);
    
    public MapNode curNode,tmpNode;
    public MapNode[] MapNodeArr;
    private GameData gameData;
    private string curFileName;
    private int stageNum;

    public TextMeshProUGUI curMapName;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        if(SceneController.Instance.isChange)
        {
            transition.cType= SceneController.Instance.colorType;
            transition.ReverseExecute();
            transition.SetColor();
            SceneController.Instance.isChange = false;
        }
        SetData();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return)||(Input.GetKeyDown(KeyCode.Space)) && !isMoving && !curNode.isEmpty))
        {
            isMoving = true;
            EnterMap(curNode.GetFileName());
            SoundBox.instance.StopBGM();
            SoundBox.instance.PlaySFX("EnterGame");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("StageSelect");
        }
        if (Input.GetKey(KeyCode.RightArrow) && !isMoving)
        {
            tmpNode = curNode.MoveToRight();
            player.GetComponent<Player>().Flip(Vector3.right);
            if (tmpNode != curNode)
            {
                isMoving = true;
                StartCoroutine(MoveCoroutine(tmpNode.transform.position));
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow) && !isMoving)
        {
            tmpNode = curNode.MoveToLeft();
            player.GetComponent<Player>().Flip(Vector3.left);
            if (tmpNode != curNode)
            {
                isMoving = true;
                StartCoroutine(MoveCoroutine(tmpNode.transform.position));
            }
        }
        if (Input.GetKey(KeyCode.UpArrow) && !isMoving)
        {
            tmpNode = curNode.MoveToUp();
            player.GetComponent<Player>().Flip(Vector3.up);
            if (tmpNode != curNode)
            {
                isMoving = true;
                StartCoroutine(MoveCoroutine(tmpNode.transform.position));
            }
        }
        if (Input.GetKey(KeyCode.DownArrow) && !isMoving)
        {
            tmpNode = curNode.MoveToDown();
            player.GetComponent<Player>().Flip(Vector3.down);
            if (tmpNode != curNode)
            {
                isMoving = true;
                StartCoroutine(MoveCoroutine(tmpNode.transform.position));
            }
        }
        
    }

    public void EnterMap(string filename)
    {
        //MapManager.Instance.fileName = filename;
        SceneController.Instance.SetSceneID(filename.Substring(4));
        gameData.lastMap[gameData.curStage-1] = curNode.id;
        DataManager.Instance.SaveData();
        curFileName = filename;
        transition.Execute();
        transition.SetColor(new Color(53f / 255f, 53f / 255f, 53f / 255f));
    }

    public void SetData()
    {
        gameData = DataManager.Instance.curData;
        int progress = gameData.mapProgress[gameData.curStage-1];
        stageNum = gameData.curStage;
        for(int i=1;i<=MapNodeArr.Length-1;++i)
        {
            MapNodeArr[i].id = i;
            MapNodeArr[i].SetFileName( "map_" + stageNum.ToString() + "_" + i.ToString());
            if (Convert.ToBoolean(progress  & (1 << i))) MapNodeArr[i].UnLock();
            else MapNodeArr[i].Lock();
        }

        curNode = MapNodeArr[gameData.lastMap[gameData.curStage-1]];
        player.transform.position = curNode.transform.position;
        string s= stageNum.ToString()+ "_" + curNode.id.ToString();
        curMapName.gameObject.GetComponent<LocalizeStringEvent>().StringReference.SetReference("New Table", s);
    }
    
    public IEnumerator MoveCoroutine(Vector3 dest)
    {
        SoundBox.instance.PlaySFX("LevelMove");

        int k = 0;
        while (k++<12)
        {
            player.transform.position = Vector2.Lerp(player.transform.position, dest, 0.3f);
            yield return ws;
        }
        player.transform.position = dest;
        curNode = tmpNode;
        isMoving = false;
        if(curNode.id!=0)
        {
            string s = stageNum.ToString() + "_" + curNode.id.ToString();
            curMapName.gameObject.GetComponent<LocalizeStringEvent>().StringReference.SetReference("New Table", s);
        }
        
    }
    
    public IEnumerator WaitTransitionCoroutine()
    {
        yield return ws5;
        isMoving = false;
    }

    public void SetCurFileName(string filename)
    {
        curFileName = filename;
    }
    public string GetCurFileName()
    {
        return curFileName;
    }
    
}

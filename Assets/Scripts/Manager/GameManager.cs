using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameManager() { }

    private static GameManager instance;
    public static GameManager Instance => instance;

    public static bool isEditor;
    
    
    public Player player;
    private bool isPaused = false;
    public bool movable = true;
    public bool processing = false;
    public bool keydowndelay = false;
    public int unitDistance= 1;
    public int turnCount;
    public Vector3 faceDir;
    
    private WaitForSeconds WSinterection = new WaitForSeconds(0.03f);
    private WaitForSeconds WSkeydowndelay = new WaitForSeconds(0.15f);
    [SerializeField]
    private List<IObject> objects,colorTiles,alphaObjs;
    private List<Palette> palettes;
    private List<Filter> filters;

    [SerializeField]
    private ColorType c1;
    [SerializeField] private ColorType c2;

    public GameObject RestartPanel,PausePanel;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetMapObjects();
    }

    private void Start()
    {
        turnCount = 0;
        faceDir=Vector3.left;
    }

    void Update()
    {
        if (isPaused) return;
        
        if (!movable && !player.isMoving&&!processing)
            StartCoroutine(InteractionCoroutine());
        
        if (!movable||keydowndelay)
            return;
            
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.left))
            {
                player.Move(Vector2.left);
                MapManager.Instance.MoveObjectsInList(Vector2.left);
            }
            faceDir=Vector2.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.right))
            {
                player.Move(Vector2.right);
                MapManager.Instance.MoveObjectsInList(Vector2.right);
            }
            faceDir=Vector2.right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.up))
            {
                player.Move(Vector2.up);
                MapManager.Instance.MoveObjectsInList(Vector2.up);
            }
            faceDir=Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.down))
            {
                player.Move(Vector2.down);
                MapManager.Instance.MoveObjectsInList(Vector2.down);
            }
            faceDir = Vector2.down;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            keydowndelay = false;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            keydowndelay = false;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            keydowndelay = false;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            keydowndelay = false;
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            if (turnCount > 0)
            {
                keydowndelay = true;
                StartCoroutine(KeyDownDelayCoroutine());
                foreach (IObject obj in objects)
                {
                    obj.Undo();
                }

                foreach (IObject obj in alphaObjs)
                {
                    obj.Undo();
                }

                for (int i=colorTiles.Count-1; i>=0;i--)
                {
                    colorTiles[i].Undo();
                }

                foreach (Filter filter in filters)
                {
                    filter.Undo();
                }

                turnCount--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isEditor)
            {
                SceneManager.LoadScene("MapEditor");
                Destroy(GameManager.Instance.gameObject);
            }
            else
            {
                PausePanel.SetActive(true);
                Pause();
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RestartPanel.SetActive(true);
            Pause();
        }
    }

    public void SetMapObjects()
    {
        if (isEditor)
        {
            MapManager.Instance.SetMapObjectsBySaveObject(CustomManager.Instance.saveList.curNode.Data);
        }
        else
        {
            string fileName = MapSelectMgr.Instance.GetCurFileName();
            string BGM_NAME = "Stage" + fileName[4]+"_BGM";
            
            MapManager.Instance.SetMapObjectsByFileName(fileName);
            SoundBox.instance.PlayBGM(BGM_NAME);
        }
        objects = MapManager.Instance.GetMapObjects();
        colorTiles = MapManager.Instance.GetColorTiles();
        palettes = MapManager.Instance.GetPalettes();
        filters = MapManager.Instance.GetFilters();
        alphaObjs = MapManager.Instance.GetAlphaObjs();
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void SaveDataAll()
    {
        foreach (IObject obj in objects)
        {
            obj.SaveData();
        }

        foreach (IObject obj in alphaObjs)
        {
            obj.SaveData();
        }

        foreach (IObject obj in colorTiles)
        {
            obj.SaveData();
        }

        foreach (Filter filter in filters)
        {
            filter.SaveData();
        }
    }

    public void ResetObjectsList()
    {
        objects.Clear();
        colorTiles.Clear();
        palettes.Clear();
        alphaObjs.Clear();
    }

    IEnumerator InteractionCoroutine()
    {
        processing = true;
        foreach (IObject obj in alphaObjs)
        {
            if(obj.gameObject.activeSelf&&MapManager.Instance.gameGrid[obj.objPos.x,obj.objPos.y].Count>=2) obj.Interaction();
        }
        
        foreach (IObject obj in objects)
        {
            if(obj.gameObject.activeSelf&&obj.GetComponent<IObject>().Type!=ObjType.Palette&&MapManager.Instance.gameGrid[obj.objPos.x,obj.objPos.y].Count>=2) obj.Interaction();
        }
        for (int i = colorTiles.Count - 1; i >= 0; i--)
        {
            if(colorTiles[i].gameObject.activeSelf&&MapManager.Instance.gameGrid[colorTiles[i].objPos.x,colorTiles[i].objPos.y].Count>=2)
                colorTiles[i].Interaction();
        }
        yield return WSinterection;
        movable = true;
        processing = false;
    }

    IEnumerator KeyDownDelayCoroutine()
    {
        yield return WSkeydowndelay;
        keydowndelay = false;
    }
    [ButtonMethod]
    void MixTest()
    {
        PCHManager.TestMixColor(c1, c2);
    }
    [ButtonMethod]
    void SubTest()
    {
        PCHManager.TestSubColor(c1, c2);
    }
}

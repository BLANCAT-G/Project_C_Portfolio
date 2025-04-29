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
    private List<IObject> objects;
    private List<IObject> colortiles;
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
            if (player.MoveCheck(Vector2.left)) player.Move(Vector2.left);
            faceDir=Vector2.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.right)) player.Move(Vector2.right);
            faceDir=Vector2.right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.up)) player.Move(Vector2.up);
            faceDir=Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SaveDataAll();
            movable = false;
            keydowndelay = true;
            turnCount++;
            StartCoroutine(KeyDownDelayCoroutine());
            if (player.MoveCheck(Vector2.down)) player.Move(Vector2.down);
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
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (turnCount > 0)
            {
                foreach (IObject obj in objects)
                {
                    obj.Undo();
                }

                for (int i=colortiles.Count-1; i>=0;i--)
                {
                    colortiles[i].Undo();
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
        if(isEditor) MapManager.Instance.SetMapObjectsByFileName(CustomManager.Instance.GetFileName());
        else
        {
            string fileName = MapSelectMgr.Instance.GetCurFileName();
            string BGM_NAME = "Stage" + fileName[4]+"_BGM";
            
            MapManager.Instance.SetMapObjectsByFileName(fileName);
            SoundBox.instance.PlayBGM(BGM_NAME);
        }
        objects = MapManager.Instance.GetMapObjects();
        colortiles = MapManager.Instance.GetColorTiles();
        palettes = MapManager.Instance.GetPalettes();
        filters = MapManager.Instance.GetFilters();
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

        foreach (IObject obj in colortiles)
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
        colortiles.Clear();
        palettes.Clear();
    }

    IEnumerator InteractionCoroutine()
    {
        processing = true;
        foreach (IObject obj in objects)
        {
            if(obj.interObj)
                obj.Interaction();
        }
        for (int i = colortiles.Count - 1; i >= 0; i--)
        {
            if (colortiles[i].interObj)
                colortiles[i].Interaction();
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

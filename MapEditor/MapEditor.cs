using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GridPos
{
    public int x { get; set; }
    public int y { get; set; }

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
public class MapEditor: MonoBehaviour
{
    public enum EditMode{
        Tile,
        Deco,
        Wall,
        FilterWall,
        Object,
        Filter,
        Lock,
        Edit,
    }
    // Start is called before the first frame update
    private bool isPause;
    
    private GridMap gridmap;
    
    private Grid<GameObject> tileVisualGrid,
        decoVisualGrid,
        wallVisualGrid,
        fwallVisualGrid,
        objVisualGrid,
        filterVisualGrid1,
        filterVisualGrid2,
        colorVisualGrid,
        lockVisualGrid;
    
    private TileInfo tilevalue;
    private DecoInfo decovalue;
    private WallInfo wallvalue;
    private FWallInfo fwallvalue;
    private ObjInfo objvalue;
    private FilterInfo filtervalue;
    private ColorType colorvalue;
    
    public EditMode editMode;
    public float cameraSpeed;
    public string loadFileName;
    public int mapStyle;
    
    private bool needUpdate;
    private bool needSave;
    private bool playerExist = false;
    private Transform Tiles, Decos, Walls, Objects, Filters;

    private static List<ObjType> NoneColorObjectsList = new List<ObjType>()
    {
        ObjType.Mop, ObjType.TrailBrush, ObjType.Water_Bucket, ObjType.Fixed_Water_Bucket,ObjType.WoodHammer
    };
    public int mapWidth, mapHeight;
    [HideInInspector]
    public Sprite[] tileSpritesArr, decoSpritesArr, wallSpritesArr,fwallSpritesArr, objSpritesArr, objBaseSpritesArr, noneColorObjSpritesArr,filterSpritesArr,stampSpritesArr;

    public GameObject defaultTile,defaultObj,defaultLock,nullTile,wallTile;
    public GridLine gridline;
    public ButtonSetter buttonSetter;
    public GameObject optionUI,objEditUI,sandEditUI,filterEditUI,lockEditUI;
    public Image outlineUI;
    public TMP_InputField widthField, heightField;
    public TextMeshProUGUI mapName;
    
    private Vector3 tmpClickPos, tmpCameraPos;

    private GridMap.SaveObject saveObject;
    private MySaveList<GridMap.SaveObject> saveList;
    private GridPos curPos;
    void Start()
    {
        outlineUI.alphaHitTestMinimumThreshold = 0.5f;
        Tiles = transform.Find("Tiles");
        Decos=transform.Find("Decos");
        Walls=transform.Find("Walls");
        Objects=transform.Find("Objects");
        Filters = transform.Find("Filters");
        InitMap();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isPause) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            editMode = EditMode.Lock;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("SampleScene");
        }
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("CustomSelect");
        }
        
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
        {
            Redo();
            CheckPlayer();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
            CheckPlayer();
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel")*5;
        Camera.main.orthographicSize -= scroll;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            needUpdate = true;
            needSave = true;
            Vector3 mousePos= getMousePos();
            if (gridmap.inBoundary(mousePos))
            {
                if (editMode == EditMode.Edit)
                {
                    needSave = false;
                    ObjInfo objInfo= gridmap.getObjValue(mousePos);
                    LockInfo lockInfo=new LockInfo(false);
                    FilterInfo filterInfo=new FilterInfo();
                    if (gridmap.inFilterBoundary(mousePos))
                    {
                        lockInfo = gridmap.getLockValue(mousePos);
                        filterInfo = gridmap.getFilterValue(mousePos);
                    }
                    if (lockInfo.exist)
                    {
                        lockEditUI.GetComponent<LockEditor>().SetObj(lockInfo);
                        lockEditUI.SetActive(true);
                        Pause();
                    }
                    else if (filterInfo.filter1.filterType != FilterType.Null || filterInfo.filter2.filterType != FilterType.Null)
                    {
                        filterEditUI.GetComponent<FilterEditor>().SetObj(filterInfo);
                        filterEditUI.SetActive(true);
                        Pause();
                    }
                    else if (objInfo.objType == ObjType.SandColor)
                    {
                        sandEditUI.GetComponent<ObjEditor>().SetObj(objInfo);
                        sandEditUI.SetActive(true);
                        Pause();
                    }
                    else if (objInfo.objType != ObjType.Null)
                    {
                        objEditUI.GetComponent<ObjEditor>().SetObj(objInfo);
                        objEditUI.SetActive(true);
                        Pause();
                    }
                }
                else if ( editMode == EditMode.Filter && gridmap.inFilterBoundary(mousePos))
                {
                    FilterInfo filterInfo = gridmap.getFilterValue(mousePos);
                    gridmap.setValue(new LockInfo(),mousePos);
                    gridmap.setValue(new FWallInfo(0),mousePos);
                    if (filterInfo.filter1.filterType == FilterType.Null && filterInfo.filter2.filterType==FilterType.Null)
                    {
                        gridmap.setValue(filtervalue.DeepCopy(), mousePos);
                    }
                    else
                    {
                        gridmap.setValue(new FilterInfo(),mousePos);
                    }
                }
                else if (editMode == EditMode.Lock && gridmap.inFilterBoundary(mousePos))
                {
                    LockInfo lockInfo = gridmap.getLockValue(mousePos);
                    gridmap.setValue(new FilterInfo(),mousePos);
                    gridmap.setValue(new FWallInfo(0),mousePos);
                    if (lockInfo.exist) gridmap.setValue(new LockInfo(), mousePos);
                    else gridmap.setValue(new LockInfo(true), mousePos);
                }
            }
            
        }
        
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            needUpdate = true;
            needSave = true;
            Vector3 mousePos= getMousePos();
            if (gridmap.inBoundary(mousePos))
            {
                switch (editMode)
                {
                    case EditMode.Tile:
                        gridmap.setValue(tilevalue.DeepCopy(),mousePos);
                        break;
                    case EditMode.Deco:
                        gridmap.setValue(decovalue.DeepCopy(),mousePos);
                        break;
                    case EditMode.Wall:
                        gridmap.setValue(wallvalue.DeepCopy(),mousePos);
                        gridmap.getObjValue(mousePos).objType = 0;
                        break;
                    case EditMode.FilterWall:
                        gridmap.setValue(fwallvalue,mousePos);
                        gridmap.setValue(new FilterInfo(),mousePos);
                        gridmap.setValue(new LockInfo(),mousePos);
                        break;
                    case EditMode.Object:
                        if (objvalue.objType == ObjType.Player)
                        {
                            if (!playerExist)
                            {
                                gridmap.setValue(objvalue.DeepCopy(),mousePos);
                                playerExist = true;
                            }
                        }
                        else
                        {
                            ObjInfo objInfo = gridmap.getObjValue(mousePos);
                            gridmap.setValue(objvalue.DeepCopy(),mousePos);
                            if (objInfo.objType == ObjType.Player) playerExist = false;
                        }
                        gridmap.getWallValue(mousePos).wallval=0;
                        break;
                        
                    default:
                        break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (needSave)
            {
                AddSaveObject();
            }
            
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            needUpdate = true;
            needSave = true;
            Vector3 mousePos= getMousePos();
            tmpClickPos = Input.mousePosition;
            tmpCameraPos = Camera.main.transform.position;
        }

        if (Input.GetMouseButton(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            Camera.main.transform.position = tmpCameraPos + Camera.main.ScreenToViewportPoint(tmpClickPos-Input.mousePosition)*cameraSpeed;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            gridmap.Save(CustomManager.Instance.GetFileName(),saveObject.mapName);
        }
    }

    private void LateUpdate()
    {
        if (needUpdate)
        {
            GridUpdate();
            needUpdate = false;
        }
    }

    public void setWH(int width, int height)
    {
        this.mapHeight = height;
        this.mapWidth = width;
    }

    public void InitGrid()
    {
        gridmap = new GridMap(mapWidth, mapHeight, new Vector3(0, 0));
        gridline.DrawGridLine(new Vector3(0,0,0),mapWidth,mapHeight,1);
        gridline.gameObject.SetActive(false);

        tileVisualGrid = new Grid<GameObject>(mapWidth, mapHeight, 1, new Vector3(0, 0));
        tilevalue = new TileInfo(0);
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                GameObject tmpObj = Instantiate(defaultTile,Tiles);
                tmpObj.GetComponent<SpriteRenderer>().sortingOrder = 0;
                tmpObj.transform.position = tileVisualGrid.getCenterPosition(x, y);
                tileVisualGrid.setValue(x,y,tmpObj);
            }
        }
        
        decoVisualGrid = new Grid<GameObject>(mapWidth, mapHeight, 1, new Vector3(0, 0));
        decovalue = new DecoInfo(0);
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                GameObject tmpObj = Instantiate(nullTile,Decos);
                tmpObj.GetComponent<SpriteRenderer>().sortingOrder = 1;
                tmpObj.transform.position = decoVisualGrid.getCenterPosition(x, y);
                decoVisualGrid.setValue(x,y,tmpObj);
            }
        }
        
        wallVisualGrid = new Grid<GameObject>(mapWidth, mapHeight, 1, new Vector3(0, 0));
        wallvalue = new WallInfo(0);
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                GameObject tmpObj = Instantiate(wallTile,Walls);
                tmpObj.GetComponent<SpriteRenderer>().sortingOrder = 2;
                tmpObj.transform.position = wallVisualGrid.getCenterPosition(x, y);
                wallVisualGrid.setValue(x,y,tmpObj);
            }
        }
        
        fwallVisualGrid=new Grid<GameObject>(mapWidth * 2 - 1, mapHeight * 2 - 1, 0.5f, new Vector3(0.25f, 0.25f));
        fwallvalue = new FWallInfo(0);
        for (int x = 0; x < mapWidth * 2 - 1; ++x) 
        {
            for (int y = 0; y < mapHeight * 2 - 1; ++y)
            {
                if ((x + y) % 2 == 0) continue;
                GameObject tmpObj = Instantiate(wallTile,Walls);
                tmpObj.GetComponent<SpriteRenderer>().sortingOrder = 3;
                tmpObj.transform.position = fwallVisualGrid.getCenterPosition(x, y);
                fwallVisualGrid.setValue(x,y,tmpObj);
            }
        }

        objVisualGrid = new Grid<GameObject>(mapWidth, mapHeight, 1, new Vector3(0, 0));
        objvalue = new ObjInfo(ObjType.Player, ColorType.Red);
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                GameObject tmpObj = Instantiate(defaultObj,Objects);
                tmpObj.transform.Find("Base").GetComponent<SpriteRenderer>().sortingOrder = 5;
                tmpObj.transform.position = objVisualGrid.getCenterPosition(x, y);
                objVisualGrid.setValue(x,y,tmpObj);
            }
        }
        
        filterVisualGrid1 = new Grid<GameObject>(mapWidth * 2 - 1, mapHeight * 2 - 1, 0.5f, new Vector3(0.25f, 0.25f));
        filterVisualGrid2 = new Grid<GameObject>(mapWidth * 2 - 1, mapHeight * 2 - 1, 0.5f, new Vector3(0.25f, 0.25f));
        filtervalue = new FilterInfo();
        for (int x = 1; x < mapWidth * 2 - 1; x+=2) // 세로 필터
        {
            for (int y = 0; y < mapHeight * 2 - 1; y+=2)
            {
                GameObject tmpSegment1 = Instantiate(defaultObj, Filters),
                    tmpSegment2 = Instantiate(defaultObj, Filters);
                
                tmpSegment1.transform.position = filterVisualGrid1.getCenterPosition(x, y);
                tmpSegment2.transform.position = filterVisualGrid2.getCenterPosition(x, y);
                tmpSegment1.GetComponent<SpriteRenderer>().sortingOrder = 7;
                tmpSegment2.GetComponent<SpriteRenderer>().sortingOrder = 7;
                tmpSegment1.transform.Rotate(0,0,90);
                tmpSegment2.transform.Rotate(0,0,-90);
                filterVisualGrid1.setValue(x,y,tmpSegment1);
                filterVisualGrid2.setValue(x,y,tmpSegment2);
            }
        }
        for (int x = 0; x < mapWidth * 2 - 1; x+=2) // 가로 필터
        {
            for (int y = 1; y < mapHeight * 2 - 1; y+=2)
            {
                GameObject tmpSegment1 = Instantiate(defaultObj, Filters),
                    tmpSegment2 = Instantiate(defaultObj, Filters);
                
                tmpSegment1.transform.position = filterVisualGrid1.getCenterPosition(x, y);
                tmpSegment2.transform.position = filterVisualGrid2.getCenterPosition(x, y);
                tmpSegment1.GetComponent<SpriteRenderer>().sortingOrder = 7;
                tmpSegment2.GetComponent<SpriteRenderer>().sortingOrder = 7;
                tmpSegment2.transform.Rotate(0,0,180);
                filterVisualGrid1.setValue(x,y,tmpSegment1);
                filterVisualGrid2.setValue(x,y,tmpSegment2);
            }
        }

        lockVisualGrid = new Grid<GameObject>(mapWidth * 2 - 1, mapHeight * 2 - 1, 0.5f, new Vector3(0.25f, 0.25f));
        for (int x = 1; x < mapWidth * 2 - 1; x+=2) 
        {
            for (int y = 0; y < mapHeight * 2 - 1; y+=2)
            {
                GameObject tmpLock = Instantiate(defaultLock,Filters);
                tmpLock.transform.position = lockVisualGrid.getCenterPosition(x, y);
                tmpLock.transform.Rotate(0,0,90);
                tmpLock.GetComponent<SpriteRenderer>().sortingOrder = 7;
                tmpLock.SetActive(false);
                lockVisualGrid.setValue(x,y,tmpLock);
            }
        }
        for (int x = 0; x < mapWidth * 2 - 1; x+=2) 
        {
            for (int y = 1; y < mapHeight * 2 - 1; y+=2)
            {
                GameObject tmpLock = Instantiate(defaultLock,Filters);
                tmpLock.transform.position = lockVisualGrid.getCenterPosition(x, y);
                tmpLock.GetComponent<SpriteRenderer>().sortingOrder = 7;
                tmpLock.SetActive(false);
                lockVisualGrid.setValue(x,y,tmpLock);
            }
        }
        
        Camera.main.transform.position = new Vector3(mapWidth / 2 , mapHeight / 2, -10);
        Camera.main.orthographicSize = Mathf.Max(mapHeight,mapWidth*9/16)*0.7f;
    }

    public void InitMap()
    {
        isPause = false;
        editMode = EditMode.Tile;
        needUpdate = false;
        needSave = false;
        saveObject = SaveSystem.LoadObject<GridMap.SaveObject>(CustomManager.Instance.GetFileName());
        mapStyle = saveObject.mapStyle;
        mapHeight = saveObject.height;
        mapWidth = saveObject.width;
        mapName.text = saveObject.mapName;
        InitSprites();
        buttonSetter.setButton();
        InitGrid();
        gridmap.Load(CustomManager.Instance.GetFileName());
        CheckPlayer();
        saveList = new MySaveList<GridMap.SaveObject>(gridmap.CreateSaveObject(), 30);
        needUpdate = true;
    }

    public void InitSprites()
    {
        tileSpritesArr=SpriteContainer.Instance.mapSpritesArray[mapStyle].tiles;
        decoSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].decos;
        wallSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].walls;
        fwallSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].filterwalls;
        objSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].objs;
        objBaseSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].objbases;
        noneColorObjSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].noneobjs;
        filterSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].filters;
        stampSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].stamps;
    }

    public void GridUpdate()
    {
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                tileVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                    tileSpritesArr[gridmap.getTileValue(x, y).tileval];
                decoVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                    decoSpritesArr[gridmap.getDecoValue(x, y).decoval];
                wallVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                    wallSpritesArr[gridmap.getWallValue(x, y).wallval];
            }
        }
        
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                ObjInfo objInfo = gridmap.getObjValue(x, y);
                GameObject tmp = objVisualGrid.getValue(x, y);
                SpriteRenderer spriteRenderer = tmp.GetComponent<SpriteRenderer>();
                if (objInfo.objType == ObjType.Stamp)
                {
                    spriteRenderer.sprite = stampSpritesArr[(int)objInfo.colorType];
                    tmp.transform.Find("Base").GetComponent<SpriteRenderer>().sprite = noneColorObjSpritesArr[0];
                    spriteRenderer.color=Color.white;
                    continue;
                }
                else if (objInfo.objType == ObjType.Player && objInfo.colorType==ColorType.None)
                {
                    spriteRenderer.sprite = objSpritesArr[(int)objInfo.objType];
                    spriteRenderer.color = ColorType.PlayerNone.ToColor();
                    tmp.transform.Find("Base").GetComponent<SpriteRenderer>().sprite = objBaseSpritesArr[(int)objInfo.objType];
                    continue;
                }else if ((objInfo.objType == ObjType.Easel || objInfo.objType == ObjType.Brush) &&
                          objInfo.colorType == ColorType.None)
                {
                    spriteRenderer.sprite = noneColorObjSpritesArr[0];
                    tmp.transform.Find("Base").GetComponent<SpriteRenderer>().sprite = noneColorObjSpritesArr[(int)objInfo.objType];
                    continue;
                }
                spriteRenderer.sprite = objSpritesArr[(int)objInfo.objType];
                spriteRenderer.color = objInfo.colorType.ToColor();
                tmp.transform.Find("Base").GetComponent<SpriteRenderer>().sprite = objBaseSpritesArr[(int)objInfo.objType];
                if (objInfo.objType == ObjType.Paint ) spriteRenderer.sortingOrder = 6;
                else spriteRenderer.sortingOrder = 4;
            }
        }
        for (int x = 1; x < mapWidth * 2 - 1; x+=2) // 세로 필터
        {
            for (int y = 0; y < mapHeight * 2 - 1; y+=2)
            {
                FilterInfo filterInfo = gridmap.getFilterValue(x, y);
                GameObject tmpSegment1 = filterVisualGrid1.getValue(x, y), tmpSegment2 = filterVisualGrid2.getValue(x, y);
                SpriteRenderer spriteRenderer1 = tmpSegment1.GetComponent<SpriteRenderer>(),spriteRenderer2=tmpSegment2.GetComponent<SpriteRenderer>();
                spriteRenderer1.sprite = filterSpritesArr[(int)filterInfo.filter1.filterType];
                spriteRenderer2.sprite = filterSpritesArr[(int)filterInfo.filter2.filterType];
                spriteRenderer1.color = filterInfo.filter1.colorType.ToColor();
                spriteRenderer2.color = filterInfo.filter2.colorType.ToColor();
            }
        }
        for (int x = 0; x < mapWidth * 2 - 1; x+=2) // 가로 필터
        {
            for (int y = 1; y < mapHeight * 2 - 1; y+=2)
            {
                FilterInfo filterInfo = gridmap.getFilterValue(x, y);
                GameObject tmpSegment1 = filterVisualGrid1.getValue(x, y), tmpSegment2 = filterVisualGrid2.getValue(x, y);
                SpriteRenderer spriteRenderer1 = tmpSegment1.GetComponent<SpriteRenderer>(),spriteRenderer2=tmpSegment2.GetComponent<SpriteRenderer>();
                spriteRenderer1.sprite = filterSpritesArr[(int)filterInfo.filter1.filterType];
                spriteRenderer2.sprite = filterSpritesArr[(int)filterInfo.filter2.filterType];
                spriteRenderer1.color = filterInfo.filter1.colorType.ToColor();
                spriteRenderer2.color = filterInfo.filter2.colorType.ToColor();
            }
        }
        for (int x = 0; x < mapWidth * 2 - 1;++x) 
        {
            for (int y = 0; y < mapHeight * 2 - 1;++y)
            {
                if ((x + y) % 2 == 0) continue;
                // 팔레트
                LockInfo lockInfo = gridmap.getLockValue(x, y);
                GameObject tmpLock = lockVisualGrid.getValue(x, y);
                if(!lockInfo.exist) tmpLock.SetActive(false);
                else
                {
                    tmpLock.SetActive(true);
                    for (int i = 0; i < 3; ++i)
                    {
                        tmpLock.transform.Find("Key" + i.ToString()).GetComponent<SpriteRenderer>().color =
                            lockInfo.KeyArr[i].ToColor();
                    }
                    
                }
                
                // 필터 벽
                fwallVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                    fwallSpritesArr[gridmap.getFWallValue(x,y).wallval];

            }
        }
    }

    public void GridUpdate(int x, int y,int k)
    {
        if (k == 0)
        {
            tileVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                tileSpritesArr[gridmap.getTileValue(x, y).tileval];
            decoVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                decoSpritesArr[gridmap.getDecoValue(x, y).decoval];
            wallVisualGrid.getValue(x, y).GetComponent<SpriteRenderer>().sprite =
                wallSpritesArr[gridmap.getWallValue(x, y).wallval];
        
            ObjInfo objInfo = gridmap.getObjValue(x, y);
            GameObject tmp = objVisualGrid.getValue(x, y);
            SpriteRenderer spriteRenderer = tmp.GetComponent<SpriteRenderer>();
            if (objInfo.objType == ObjType.Stamp)
            {
                spriteRenderer.sprite = stampSpritesArr[(int)objInfo.colorType];
                spriteRenderer.color=Color.white;
                return;
            }
            else if (objInfo.objType == ObjType.Player && objInfo.colorType==ColorType.None)
            {
                spriteRenderer.sprite = objSpritesArr[(int)objInfo.objType];
                spriteRenderer.color = ColorType.PlayerNone.ToColor();
                tmp.transform.Find("Base").GetComponent<SpriteRenderer>().sprite = objBaseSpritesArr[(int)objInfo.objType];
                return;
            }
            spriteRenderer.sprite = objSpritesArr[(int)objInfo.objType];
            spriteRenderer.color = objInfo.colorType.ToColor();
            tmp.transform.Find("Base").GetComponent<SpriteRenderer>().sprite = objBaseSpritesArr[(int)objInfo.objType];
            if (objInfo.objType == ObjType.Paint || objInfo.objType == ObjType.Brush) spriteRenderer.sortingOrder = 6;
            else spriteRenderer.sortingOrder = 4;
        }
        else if(k==1)
        {
            if ((x + y) % 2 == 0) return;
            
            FilterInfo filterInfo = gridmap.getFilterValue(x, y);
            GameObject tmpSegment1 = filterVisualGrid1.getValue(x, y), tmpSegment2 = filterVisualGrid2.getValue(x, y);
            SpriteRenderer spriteRenderer1 = tmpSegment1.GetComponent<SpriteRenderer>(),spriteRenderer2=tmpSegment2.GetComponent<SpriteRenderer>();
            spriteRenderer1.sprite = filterSpritesArr[(int)filterInfo.filter1.filterType];
            spriteRenderer2.sprite = filterSpritesArr[(int)filterInfo.filter2.filterType];
            spriteRenderer1.color = filterInfo.filter1.colorType.ToColor();
            spriteRenderer2.color = filterInfo.filter2.colorType.ToColor();
            
            filterInfo = gridmap.getFilterValue(x, y);
            tmpSegment1 = filterVisualGrid1.getValue(x, y);
            tmpSegment2 = filterVisualGrid2.getValue(x, y);
            spriteRenderer1 = tmpSegment1.GetComponent<SpriteRenderer>();
            spriteRenderer2 = tmpSegment2.GetComponent<SpriteRenderer>();
            spriteRenderer1.sprite = filterSpritesArr[(int)filterInfo.filter1.filterType];
            spriteRenderer2.sprite = filterSpritesArr[(int)filterInfo.filter2.filterType];
            spriteRenderer1.color = filterInfo.filter1.colorType.ToColor();
            spriteRenderer2.color = filterInfo.filter2.colorType.ToColor();

            LockInfo lockInfo = gridmap.getLockValue(x, y);
            GameObject tmpLock = lockVisualGrid.getValue(x, y);
            if(!lockInfo.exist) tmpLock.SetActive(false);
            else
            {
                tmpLock.SetActive(true);
                for (int i = 0; i < 3; ++i)
                {
                    tmpLock.transform.Find("Key" + i.ToString()).GetComponent<SpriteRenderer>().color =
                        lockInfo.KeyArr[i].ToColor();
                }
                    
            }
        }
        
    }

    public void CheckPlayer()
    {
        playerExist = false;
        for (int x = 0; x < mapWidth; ++x) for (int y = 0; y < mapHeight; ++y) if (gridmap.getObjValue(x, y).objType == ObjType.Player) playerExist = true;
    }

    public void Resize(int w, int h)
    {
        if (mapWidth == w && mapHeight == h) return;
        mapWidth = w;
        mapHeight = h;
        foreach (Transform t in Tiles) Destroy(t.gameObject);
        foreach (Transform t in Decos) Destroy(t.gameObject);
        foreach (Transform t in Walls) Destroy(t.gameObject);
        foreach (Transform t in Objects) Destroy(t.gameObject);
        foreach (Transform t in Filters) Destroy(t.gameObject);
   
        InitGrid();
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        isPause = false;
    }

    public void Undo()
    {
        saveList.Undo();
        saveObject = saveList.curNode.Data;
        Resize(saveObject.width,saveObject.height);
        gridmap.Load(saveObject);
        needUpdate = true;
    }

    public void Redo()
    {
        saveList.Redo();
        saveObject = saveList.curNode.Data;
        Resize(saveObject.width,saveObject.height);
        gridmap.Load(saveObject);
        needUpdate = true;
    }

    public void ChangeProperty(int w,int h)
    {
        Resize(w,h);
        gridmap.Load(saveObject);
        AddSaveObject();
        needUpdate = true;
    }

    public void AddSaveObject()
    {
        saveList.AddSaveObject(gridmap.CreateSaveObject());
        saveObject = saveList.curNode.Data;
        needSave = false;
    }

    public void setTileValue(TileInfo tilevalue)
    {
        this.tilevalue = tilevalue;
    }

    public void setDecoValue(DecoInfo decovalue)
    {
        this.decovalue = decovalue;
    }

    public void setWallValue(WallInfo wallvalue)
    {
        this.wallvalue = wallvalue;
    }

    public void setFWallValue(FWallInfo fwallvalue)
    {
        this.fwallvalue = fwallvalue;
    }

    public void setObjectValue(ObjInfo objvalue)
    {
        this.objvalue = objvalue;
    }
    
    public void setFilterValue(FilterInfo filtervalue)
    {
        this.filtervalue = filtervalue;
    }

    public void setColorValue(ColorType colorvalue)
    {
        this.colorvalue = colorvalue;
        if (editMode == EditMode.Object) objvalue.colorType = colorvalue;
        else if (editMode == EditMode.Filter) filtervalue.filter1.colorType = filtervalue.filter2.colorType=colorvalue;
    }

    public void NeedUpdate()
    {
        needUpdate = true;
    }

    public ColorType getColorValue()
    {
        return colorvalue;
    }

    public void Save(string fileName,string mapName)
    {
        gridmap.Save(fileName, mapName);
    }

    public static Vector3 getMousePos()
    {
        Vector3 vec = getMousePosWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 getMousePosWithZ(Vector3 screenPos, Camera mainCamera)
    { 
        return mainCamera.ScreenToWorldPoint(screenPos);
    }
    
    //UI

    public void OnOptionButtonClick()
    {
        Pause();
        optionUI.SetActive(true);
        widthField.text = mapWidth.ToString();
        heightField.text = mapHeight.ToString();
        
    }
    public void OnApplyButtonClick()
    {
        ChangeProperty(int.Parse(widthField.text),int.Parse(heightField.text));
        optionUI.SetActive(false);
        Resume();
    }
}

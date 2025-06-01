using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    private MapManager() { }

    private static MapManager instance;
    public static MapManager Instance => instance;
    
    private GridMap gridMap;
    public Tilemap tilemap, wallmap;

    public List<GameObject>[,] gameGrid;
    public bool[,] wallGrid;
    public List<GameObject> moveObjList=new List<GameObject>();
    public int mapWidth, mapHeight, mapStyle;
    public string fileName;
    public GameObject obj,WallTile,defaultTile,defaultLock,defaultColorTile,background,bgDeco,keyguide;
    public GameObject[] ObjectsArr;
    public GameObject[] FiltersArr;
    public GameObject wall,filterwall;
    
    
    private Sprite[] tileSpritesArr,decoSpritesArr,wallSpritesArr,fwallSpritesArr,stampSpritesArr;
    private RuleTile ruletile, rulewall;
    private TileInfo tileInfo;
    private DecoInfo decoInfo;
    private WallInfo wallInfo;
    private List<IObject> mapObjects;
    private List<IObject> colorTiles,alphaObjs;
    private List<Filter> fiters;
    private List<Palette> palettes;

    public Camera renderCam;
    public CenterZoom CameraZoom;
    public RawImage display;
    
    private static List<ObjType> NoneColorObjectsList = new List<ObjType>()
    {
        ObjType.Mop, ObjType.TrailBrush, ObjType.Water_Bucket, ObjType.Fixed_Water_Bucket,ObjType.WoodHammer,ObjType.Easel,ObjType.BlackSmoke,ObjType.Eraser
    };

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

    public List<IObject> GetMapObjects()
    {
        return mapObjects;
    }

    public List<IObject> GetColorTiles()
    {
        return colorTiles;
    }

    public List<Palette> GetPalettes()
    {
        return palettes;
    }

    public List<Filter> GetFilters()
    {
        return fiters;
    }

    public List<IObject> GetAlphaObjs()
    {
        return alphaObjs;
    }

    public void SetMapObjectsByFileName(string fileName)
    {
        GridMap.SaveObject saveObject = SaveSystem.LoadObject<GridMap.SaveObject>(fileName);
        SetMapObjectsBySaveObject(saveObject);
    }
    
    public void SetMapObjectsBySaveObject(GridMap.SaveObject saveObject)
    {
        mapObjects = new List<IObject>();
        colorTiles = new List<IObject>();
        palettes = new List<Palette>();
        fiters = new List<Filter>();
        alphaObjs = new List<IObject>();
        
        mapHeight = saveObject.height;
        mapWidth = saveObject.width;
        mapStyle = saveObject.mapStyle;
        ruletile = SpriteContainer.Instance.mapSpritesArray[mapStyle].ruleTile;
        rulewall = SpriteContainer.Instance.mapSpritesArray[mapStyle].ruleWall;
        wallSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].walls;
        decoSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].decos;
        fwallSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].filterwalls;
        stampSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].stamps;
        gridMap = new GridMap(mapWidth, mapHeight, new Vector3(0, 0));
        gameGrid = new List<GameObject>[mapWidth * 2 - 1, mapHeight * 2 - 1];
        wallGrid = new bool[mapWidth * 2 - 1, mapHeight * 2 - 1];
        for (int x = 0; x < mapWidth*2-1; ++x) for (int y = 0; y < mapHeight * 2 - 1; ++y)
        {
            gameGrid[x, y] = new List<GameObject>();
            wallGrid[x, y] = false;
        }
        
        gridMap.Load(saveObject);

        for (int x = -1; x <= mapWidth; ++x)
        {
            for (int y = -1; y <= mapHeight; ++y)
            {
                if (x == -1 || x == mapWidth || y == -1 || y == mapHeight)
                {
                    wallmap.SetTile(new Vector3Int(x,y,0),rulewall);
                    wallmap.SetColor(new Vector3Int(x,y,0),Color.clear);
                    continue;
                }
                
                tileInfo = gridMap.getTileValue(x, y);

                if(tileInfo.tileval!=0) tilemap.SetTile(new Vector3Int(x,y,0),ruletile);
                
                decoInfo = gridMap.getDecoValue(x, y);
                if (decoInfo.decoval != 0)
                {
                    obj = Instantiate(defaultTile);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                    obj.GetComponent<SpriteRenderer>().sprite = decoSpritesArr[decoInfo.decoval];
                }

                wallInfo = gridMap.getWallValue(x, y);
                if (wallInfo.wallval == 1)
                {
                    obj = Instantiate(wall);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                    wallGrid[x * 2, y * 2] = true;
                    wallmap.SetTile(new Vector3Int(x,y,0),rulewall);
                }
                else if (wallInfo.wallval != 0)
                {
                    obj = Instantiate(WallTile);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                    obj.GetComponent<SpriteRenderer>().sprite = wallSpritesArr[wallInfo.wallval];
                    obj = Instantiate(wall);
                    wallmap.SetTile(new Vector3Int(x,y,0),rulewall);
                    if(mapStyle==0) wallmap.SetColor(new Vector3Int(x,y,0),Color.clear);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                    wallGrid[x * 2, y * 2] = true;
                }
                
            }
        }

        for (int x = 0; x < mapWidth * 2 - 1; ++x)
        {
            for (int y = 0; y < mapHeight * 2 - 1; ++y)
            {
                if ((x + y) % 2 == 0) continue;
                
                FWallInfo fwallInfo = gridMap.getFWallValue(x, y);
                if (fwallInfo.wallval != 0)
                {
                    obj = Instantiate(WallTile);
                    obj.transform.position=new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                    obj.GetComponent<SpriteRenderer>().sprite = fwallSpritesArr[fwallInfo.wallval];
                    obj = Instantiate(filterwall);
                    obj.transform.position=new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                    gameGrid[x,y].Add(obj);
                }
            }
        }
        
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                ObjInfo objInfo = gridMap.getObjValue(x, y);
                
                if (objInfo.objType == ObjType.Null) continue;
                obj = Instantiate(ObjectsArr[(int)objInfo.objType]);
                obj.transform.position = new Vector3(x + 0.5f, y + 0.5f,0);
                obj.GetComponent<IObject>().isAlpha = objInfo.isAlpha;
                obj.GetComponent<IObject>().objPos = new GridPos(x * 2, y * 2);
                gameGrid[x * 2, y * 2].Add(obj.gameObject);
                if (objInfo.objType == ObjType.Stamp)
                {
                    obj.GetComponent<SpriteRenderer>().sprite = stampSpritesArr[(int)objInfo.colorType];
                    obj.GetComponent<IObject>().colorType = objInfo.colorType;
                }
                else if (!NoneColorObjectsList.Contains(objInfo.objType)) 
                {
                    obj.GetComponent<IObject>().colorType = objInfo.colorType;
                    obj.GetComponent<IObject>().ColorChange(objInfo.colorType);
                    if (objInfo.objType == ObjType.SandColor) obj.GetComponent<SandColor>().count = (uint)objInfo.sandCount;
                }

                if (objInfo.objType == ObjType.Player)
                {
                    GameManager.Instance.player = obj.GetComponent<Player>();
                }
                
                if(objInfo.objType==ObjType.Tile) colorTiles.Add(obj.GetComponent<IObject>());
                else if(objInfo.objType==ObjType.BlackSmoke || objInfo.objType==ObjType.Eraser || objInfo.objType==ObjType.Roller || objInfo.objType==ObjType.InkStone) 
                    alphaObjs.Add(obj.GetComponent<IObject>());
                else mapObjects.Add(obj.GetComponent<IObject>());
            }
        }
        for (int x = 1; x < mapWidth * 2 - 1; x+=2) // 세로 필터
        {
            for (int y = 0; y < mapHeight * 2 - 1; y+=2)
            {
                FilterInfo filterInfo = gridMap.getFilterValue(x, y);
                if (filterInfo.filter1.filterType != FilterType.Null)
                {
                    GameObject filter = Instantiate(FiltersArr[1]);
                    filter.transform.position = new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                    Filter filterCode = filter.GetComponent<Filter>();
                    filterCode.filterType = filterInfo.filter1.filterType;
                    filterCode.colorType = filterInfo.filter1.colorType;
                    filterCode.directionType = DirectionType.Left;
                    filterCode.isAlpha = filterInfo.filter1.isAlpha;
                    if (filterInfo.filter1.isOneTime) filterCode.isOneTime = true;
                    else filterCode.isOneTime = false;
                    filterCode.ColorChange(filterInfo.filter1.colorType);
                    filterCode.setDirection();
                    fiters.Add(filterCode);
                    gameGrid[x,y].Add(filter);
                }
                if (filterInfo.filter2.filterType != FilterType.Null)
                {
                    GameObject filter = Instantiate(FiltersArr[1]);
                    filter.transform.position = new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                    Filter filterCode = filter.GetComponent<Filter>();
                    filterCode.filterType = filterInfo.filter2.filterType;
                    filterCode.colorType = filterInfo.filter2.colorType;
                    filterCode.directionType = DirectionType.Right;
                    filterCode.isAlpha = filterInfo.filter2.isAlpha;
                    if (filterInfo.filter2.isOneTime) filterCode.isOneTime = true;
                    else filterCode.isOneTime = false;
                    filterCode.ColorChange(filterInfo.filter2.colorType);
                    filterCode.setDirection();
                    fiters.Add(filterCode);
                    gameGrid[x,y].Add(filter);
                }
            }
        }
        for (int x = 0; x < mapWidth * 2 - 1; x+=2) // 가로 필터
        {
            for (int y = 1; y < mapHeight * 2 - 1; y+=2)
            {
                FilterInfo filterInfo = gridMap.getFilterValue(x, y);
                if (filterInfo.filter1.filterType != FilterType.Null)
                {
                    GameObject filter = Instantiate(FiltersArr[1]);
                    filter.transform.position = new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                    Filter filterCode = filter.GetComponent<Filter>();
                    filterCode.filterType = filterInfo.filter1.filterType;
                    filterCode.colorType = filterInfo.filter1.colorType;
                    filterCode.directionType = DirectionType.Up;
                    filterCode.isAlpha = filterInfo.filter1.isAlpha;
                    if (filterInfo.filter1.isOneTime) filterCode.isOneTime = true;
                    else filterCode.isOneTime = false;
                    filterCode.ColorChange(filterInfo.filter1.colorType);
                    filterCode.setDirection();
                    fiters.Add(filterCode);
                    gameGrid[x,y].Add(filter);
                }
                if (filterInfo.filter2.filterType != FilterType.Null)
                {
                    GameObject filter = Instantiate(FiltersArr[1]);
                    filter.transform.position = new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                    Filter filterCode = filter.GetComponent<Filter>();
                    filterCode.filterType = filterInfo.filter2.filterType;
                    filterCode.colorType = filterInfo.filter2.colorType;
                    filterCode.directionType = DirectionType.Down;
                    filterCode.isAlpha = filterInfo.filter2.isAlpha;
                    if (filterInfo.filter2.isOneTime) filterCode.isOneTime = true;
                    else filterCode.isOneTime = false;
                    filterCode.ColorChange(filterInfo.filter2.colorType);
                    filterCode.setDirection();
                    fiters.Add(filterCode);
                    gameGrid[x,y].Add(filter);
                }
            }
        }
        
        for (int x = 0; x < mapWidth * 2 - 1; ++x) 
        {
            for (int y = 0; y < mapHeight * 2 - 1; ++y)
            {
                if ((x + y) % 2 == 0) continue;
                LockInfo lockInfo = gridMap.getLockValue(x, y);
                if (!lockInfo.exist) continue;
                GameObject tmpLock = Instantiate(defaultLock);
                palettes.Add(tmpLock.GetComponent<Palette>());
                tmpLock.transform.position=new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                if(x%2==1) tmpLock.transform.Rotate(0,0,90);
                tmpLock.GetComponent<Palette>().KeyArr = lockInfo.KeyArr;
                gameGrid[x,y].Add(tmpLock);
                for (int i = 0; i < 3; ++i)
                {
                    tmpLock.transform.Find("Key" + i.ToString()).GetComponent<SpriteRenderer>().color = lockInfo.KeyArr[i].ToColor();
                }
                mapObjects.Add(tmpLock.GetComponent<IObject>());
            }
        }

        //Camera.main.orthographicSize = Mathf.Ceil(Mathf.Max(mapHeight, mapWidth * 9 / 16) * 0.55f);
        Camera.main.orthographicSize = 11.25f;
        Camera.main.transform.position = new Vector3(mapWidth/2+0.5f,mapHeight/2+0.5f, -10);
        CameraZoom.zoomRatio = 0.5f+Mathf.Min(1f,Mathf.Max(0,Mathf.Max(mapHeight, mapWidth * 9f / 16f)-10f)/10f);
        renderCam.orthographicSize = 11.25f;
        renderCam.transform.position = new Vector3(mapWidth / 2 + 0.5f, mapHeight / 2 + 0.5f, -10);
        //ChangeRenderTargetSize(8f);
        background.GetComponent<SpriteRenderer>().sprite =
            SpriteContainer.Instance.mapSpritesArray[mapStyle].background;
        background.transform.position = new Vector3(mapWidth / 2+0.5f, mapHeight / 2+0.5f, 0);
        if (mapStyle == 0)
        {
            bgDeco.SetActive(true);
            bgDeco.transform.position=new Vector3(mapWidth / 2+0.5f, mapHeight / 2+0.5f, 0);
        }

        if (fileName.Equals("map_1_1") || fileName.Equals("map_1_2"))
        {
            keyguide.SetActive(true);
            keyguide.transform.position = new Vector3(mapWidth / 2 + 0.5f, -0.6f);
        }
    }
   

    public void MoveObjectsInList(Vector3 dir)
    {
        int dx = (int)dir.x * 2, dy = (int)dir.y * 2;
        foreach (GameObject obj in moveObjList)
        {
            IObject io = obj.GetComponent<IObject>();
            gameGrid[io.objPos.x, io.objPos.y].Remove(obj);
            io.objPos = new GridPos(io.objPos.x + dx, io.objPos.y + dy);
            gameGrid[io.objPos.x, io.objPos.y].Add(obj);
        }

        moveObjList = new List<GameObject>();
    }

    public float RoundToNearestPixel(float pos)
    {
        float screenPixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);
        float pixelValue = Mathf.Round(pos * screenPixelsPerUnit);

        return pixelValue / screenPixelsPerUnit;
    }
    
    

    public void AddColorTile(IObject io)
    {
        colorTiles.Add(io);
    }

    public void DeleteColorTile(IObject io)
    {
        colorTiles.Remove(io);
    }
    

    public void ChangeRenderTargetSize(float halfSize)
    {
        // orthographicSize = 화면 반 높이(유닛) → 높이 픽셀 = sizeHalfUnits * 2 * PPU
        float rtHeight = halfSize * 2 * 32;
        // 화면 종횡비 그대로 유지
        int rtWidth = Mathf.RoundToInt(rtHeight * (float)Screen.width / Screen.height);

        // 기존 RT 해제
        if (renderCam.targetTexture != null)
            renderCam.targetTexture.Release();

        // 새 RT 생성
        //var rt = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 16);
        var rt = new RenderTexture(rtWidth, (int)rtHeight, 16);
        Debug.Log(rtWidth);
        Debug.Log((int)rtHeight);
        rt.filterMode = FilterMode.Point;      // nearest-neighbor
        rt.useMipMap = true;

        renderCam.targetTexture = rt;
        display.texture = rt;
    }
}

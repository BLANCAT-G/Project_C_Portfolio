using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private MapManager() { }

    private static MapManager instance;
    public static MapManager Instance => instance;
    
    private GridMap gridMap;
    private int mapWidth, mapHeight, mapStyle;
    public string fileName;
    public GameObject obj,WallTile,defaultTile,defaultLock,defaultColorTile,background;
    public GameObject[] ObjectsArr;
    public GameObject[] FiltersArr;
    public GameObject wall,filterwall;
    
    
    private Sprite[] tileSpritesArr,decoSpritesArr,wallSpritesArr,fwallSpritesArr,stampSpritesArr;
    private TileInfo tileInfo;
    private DecoInfo decoInfo;
    private WallInfo wallInfo;
    private List<IObject> mapObjects;
    private List<IObject> colorTiles;
    private List<Filter> fiters;
    private List<Palette> palettes;
    
    
    
    
    
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
    

    public void SetMapObjectsByFileName(string fileName)
    {
        this.fileName = fileName;
        
        
        mapObjects = new List<IObject>();
        colorTiles = new List<IObject>();
        palettes = new List<Palette>();
        fiters = new List<Filter>();
        
        GridMap.SaveObject saveObject = SaveSystem.LoadObject<GridMap.SaveObject>(fileName);
        mapHeight = saveObject.height;
        mapWidth = saveObject.width;
        mapStyle = saveObject.mapStyle;
        tileSpritesArr=SpriteContainer.Instance.mapSpritesArray[mapStyle].tiles;
        decoSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].decos;
        wallSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].walls;
        fwallSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].filterwalls;
        stampSpritesArr = SpriteContainer.Instance.mapSpritesArray[mapStyle].stamps;
        gridMap = new GridMap(mapWidth, mapHeight, new Vector3(0, 0));
        gridMap.Load(saveObject);
        
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                tileInfo = gridMap.getTileValue(x, y);
                obj = Instantiate(defaultTile);
                obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                obj.GetComponent<SpriteRenderer>().sprite = tileSpritesArr[tileInfo.tileval];
                obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
                
                decoInfo = gridMap.getDecoValue(x, y);
                if (decoInfo.decoval != 0)
                {
                    obj = Instantiate(defaultTile);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                    obj.GetComponent<SpriteRenderer>().sprite = decoSpritesArr[decoInfo.decoval];
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }

                wallInfo = gridMap.getWallValue(x, y);
                if (wallInfo.wallval != 0)
                {
                    obj = Instantiate(WallTile);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
                    obj.GetComponent<SpriteRenderer>().sprite = wallSpritesArr[wallInfo.wallval];
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    obj = Instantiate(wall);
                    obj.transform.position=new Vector3(x + 0.5f, y + 0.5f,0);
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
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    obj = Instantiate(filterwall);
                    obj.transform.position=new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
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
                }
            }
        }
        
        for (int x = 0; x < mapWidth * 2 - 1; ++x) 
        {
            for (int y = 1; y < mapHeight * 2 - 1; ++y)
            {
                if ((x + y) % 2 == 0) continue;
                LockInfo lockInfo = gridMap.getLockValue(x, y);
                if (!lockInfo.exist) continue;
                GameObject tmpLock = Instantiate(defaultLock);
                palettes.Add(tmpLock.GetComponent<Palette>());
                tmpLock.transform.position=new Vector3(x * 0.5f + 0.5f, y * 0.5f + 0.5f, 0);
                if(x%2==1) tmpLock.transform.Rotate(0,0,90);
                tmpLock.GetComponent<Palette>().KeyArr = lockInfo.KeyArr;
                for (int i = 0; i < 3; ++i)
                {
                    tmpLock.transform.Find("Key" + i.ToString()).GetComponent<SpriteRenderer>().color = lockInfo.KeyArr[i].ToColor();
                }
                mapObjects.Add(tmpLock.GetComponent<IObject>());
            }
        }
        Camera.main.orthographicSize = Mathf.Ceil(Mathf.Max(mapHeight,mapWidth*9/16)*0.55f);
        Camera.main.transform.position = new Vector3(RoundToNearestPixel(mapWidth / 2 +0.5f), RoundToNearestPixel(mapHeight / 2+0.5f), -10);
        background.GetComponent<SpriteRenderer>().sprite =
            SpriteContainer.Instance.mapSpritesArray[mapStyle].background;
        background.transform.position = new Vector3(mapWidth / 2, mapHeight / 2, 0);
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
    
}

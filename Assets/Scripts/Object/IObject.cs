using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using MyBox;
using TMPro;

public class ObjData
{
    public GridPos objPos;
    public Vector3 faceDir;
    public bool isActive;
    public bool isAcryl;
    public bool isAlpha;
    public bool isBrush;
    public bool isSuperAcryl;
    public bool isTrail;
    public uint sandCount;
    public ColorType colorType;
    public ColorType acrylColor;
    
    public ObjData(GridPos pos, Vector3 faceDir,bool isActive, bool isAcryl, bool isAlpha, bool isBrush, bool isSuperAcryl,bool isTrail,uint sandCount, ColorType colorType,ColorType acrylColor)
    {
        this.objPos = pos;
        this.faceDir = faceDir;
        this.isActive = isActive;
        this.isAlpha = isAlpha;
        this.isAcryl = isAcryl;
        this.isBrush = isBrush;
        this.isTrail = isTrail;
        this.isSuperAcryl = isSuperAcryl;
        this.sandCount = sandCount;
        this.colorType = colorType;
        this.acrylColor = acrylColor;
    }
}

public enum ObjType
{
    Null=0,
    Player=1,
    Tile,
    Fixed_Paint,
    Paint,
    Canvas,
    Acryl,
    Water_Bucket,
    Fixed_Water_Bucket,
    Brush,
    TrailBrush,
    SandColor,
    Sponge,
    Mop,
    WoodHammer,
    Stamp,
    Easel,
    BlackSmoke,
    Eraser,
    InkStone,
    Roller,
    
    Filter=100,
    Palette,
}

public abstract class IObject : MonoBehaviour
{
    private bool IsAcryl;
    [Header("Type")]
    public ObjType Type;

    
    public bool isAcryl
    {
        get { return IsAcryl; }
        set
        {
            IsAcryl = value;
            AcrylEff.SetActive(value);
            AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }
    }
    [Header("Property")]
    public bool isAlpha;
    public bool isBrush;
    public bool is3D;
    public bool isMoving;
    public bool isSuperAcryl = false;
    public bool isTrail;
    public ColorType colorType;
    public ColorType acrylColor;
    public GridPos objPos;

    private List<ObjType> NoneColorObjList =
        new List<ObjType>()
        {
            ObjType.Mop,
            ObjType.TrailBrush,
            ObjType.Water_Bucket,
            ObjType.WoodHammer,
            ObjType.Fixed_Water_Bucket,
            ObjType.BlackSmoke,
            ObjType.Eraser,
            ObjType.InkStone,
            ObjType.Roller
        };
    protected List<GameObject> hit;
    protected GameObject hitFilter, hitOppositeFilter,hitPalette,hitFwall;
    private WaitForSeconds ws = new WaitForSeconds(0.01f);

    [Header("Log")] 
    public Stack<ObjData> moveLog;
    
    public SpriteRenderer spriter;

    [Header("Interactable")]
    public IObject interObj;

    
    public GameObject AcrylEff,TrailEff,BrushEff;
    [SerializeField]
    protected TextMeshProUGUI sandCountText;
    public uint sandCount = 0;
    protected ColorType prevColor;
    public abstract void Interaction();
    //public abstract void Interact();
    
    public virtual void Awake()
    {
        moveLog = new Stack<ObjData>();
        spriter = gameObject.transform.Find("Base").GetComponent<SpriteRenderer>();
        ColorChange(colorType);
    }

    public virtual bool MoveCheck(Vector3 dir)
    {
        if (isMoving) return false;
        int dx = (int)dir.x, dy = (int)dir.y;
        if (!CheckInGrid(new GridPos(objPos.x + dx * 2, objPos.y + dy * 2))) return false;
        SetHitObjects(dir);
        if (hitFwall) return false;
        if (hitPalette && hitPalette.activeSelf) return false;
        else if (hitFilter )
        {
            Filter filter = hitFilter.GetComponent<Filter>();
            if (filter && ((filter.isOneTime && filter.used) || (filter.filterType==FilterType.None && (filter.colorType != colorType || filter.isAlpha!=isAlpha))))
            {
                return false;
            }
            
        }
        else if (hitOppositeFilter )
        {
            return false;
        }

        foreach (GameObject h in hit)
        {
            if (!h.activeSelf) continue;
            if (h.CompareTag("Wall"))
                return false;
            
            IObject obj = h.GetComponent<IObject>();
            
            if ( obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl) && obj.isAlpha==isAlpha)
            {
                if (obj.MoveCheck(dir) == false)
                    return false;
            }
        }

        return true;
    }

    public virtual void Move(Vector3 dir)
    {
        if (hitFilter && hitFilter.GetComponent<Filter>().isAlpha==isAlpha)
        {
            Filter filter = hitFilter.GetComponent<Filter>();
            if (filter.isOneTime)
            {
                filter.Use();
            }

            if (filter.filterType == FilterType.Mix && filter.isAlpha==isAlpha)
            {
                colorType = PCHManager.MixColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
            else if (filter.filterType == FilterType.Substract && filter.isAlpha==isAlpha)
            {
                colorType = PCHManager.SubstractColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
        }
        if (Type != ObjType.SandColor)
        {
            if (sandCount > 1)
            {
                sandCount--;
                sandCountText.text = sandCount.ToString();
            }
            else if (sandCountText.gameObject.activeSelf && sandCount == 1)
            {
                sandCountText.gameObject.SetActive(false);
                colorType = prevColor;
                ColorChange(colorType);
            }
        }
        

        if (isSuperAcryl)
        {
            foreach (GameObject h in hit)
            {
                if (!h.activeSelf) continue;
                IObject obj = h.gameObject.GetComponent<IObject>();
                if (obj.is3D)
                {
                    if (!NoneColorObjList.Contains(obj.Type))
                    {
                        obj.colorType = this.acrylColor;
                        obj.ColorChange(this.colorType);
                    }
                    obj.acrylColor = this.acrylColor;
                    obj.StateUpdate();
                }
            }
        }
        
        foreach (GameObject h in hit)
        {
            if (!h.activeSelf) continue;
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (h.CompareTag("Wall")) continue;
            if ( obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl) && obj.isAlpha==isAlpha)
            {
                obj.Move(dir);
            }
        }
        MapManager.Instance.moveObjList.Add(this.gameObject);
        
        isMoving = true;
        StartCoroutine(MoveCoroutine(new GridPos(objPos.x + (int)dir.x*2, objPos.y + (int)dir.y*2)));
    }
    
    protected IEnumerator MoveCoroutine(GridPos destPos)
    {
        Vector3 dest = destPos.getFilterCenterPos(Vector2.zero, 1);
        int k = 0;
        while (k++<6)
        {
            transform.position = Vector2.Lerp(transform.position, dest, 0.3f);
            yield return ws;
        }
        transform.position = dest;
        isMoving = false;
    }

    public virtual void ColorChange(ColorType cT)
    {
        colorType = cT;
        if (isSuperAcryl)
        {
            if (cT == ColorType.None)
            {
                isSuperAcryl = false;
                AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = new Color(221f/255f, 221f / 255f, 221f / 255f, 1);
            }
            else
            {
                acrylColor = cT;
                AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
            }
        }
        if (!NoneColorObjList.Contains(Type))
        {
            colorType = cT;
            spriter.color = cT.ToColor();
        }
        
                
        if(isAlpha) OnAlpha();
        else OffAlpha();
    }

    public virtual void SaveData()
    {
        ObjData newData = new ObjData(new GridPos(objPos.x,objPos.y), Vector3.left, gameObject.activeSelf, isAcryl, isAlpha, isBrush, isSuperAcryl,isTrail,sandCount,colorType,acrylColor);
        moveLog.Push(newData);
    }

    public virtual void Undo()
    {
        if (moveLog.Count == 0)
        {
            MapManager.Instance.gameGrid[objPos.x, objPos.y].Remove(gameObject);
            Destroy(gameObject);
            return;
        }
        ObjData lastData = moveLog.Pop();
        gameObject.SetActive(lastData.isActive);
        isAcryl = lastData.isAcryl;
        isAlpha = lastData.isAlpha;
        isBrush = lastData.isBrush;
        isSuperAcryl = lastData.isSuperAcryl;
        isTrail = lastData.isTrail;
        colorType = lastData.colorType;
        acrylColor = lastData.acrylColor;
        sandCount = lastData.sandCount;
        ColorChange(colorType);
        StateUpdate();
        if (!objPos.Compare(lastData.objPos))
        {
            MapManager.Instance.gameGrid[objPos.x, objPos.y].Remove(this.gameObject);
            objPos = new GridPos(lastData.objPos);
            MapManager.Instance.gameGrid[objPos.x,objPos.y].Add(this.gameObject);
            StartCoroutine(MoveCoroutine(objPos));
        }
    }

    public void StateUpdate()
    {
        if (isSuperAcryl)
        {
            AcrylEff.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
        }   
        if (TrailEff)
        {
            if(isTrail) TrailEff.SetActive(true);
            else TrailEff.SetActive(false);
        }
        if (BrushEff)
        {
            if(isBrush) BrushEff.SetActive(true);
            else BrushEff.SetActive(false);
        }
        if (sandCountText && Type!=ObjType.SandColor )
        {
            if (sandCount > 0)
            {
                sandCountText.gameObject.SetActive(true);
                sandCountText.text = sandCount.ToString();
            }
            else
            {
                sandCountText.gameObject.SetActive(false);
            }
        } 
    }

    public bool CheckInGrid(GridPos gridPos)
    {
        int gridWidth = MapManager.Instance.mapWidth * 2 - 1;
        int gridHeight = MapManager.Instance.mapHeight * 2 - 1;
        if (gridPos.x >= 0 && gridPos.x <= gridWidth && gridPos.y >= 0 && gridPos.y <= gridHeight) return true;
        return false;
    }

    public void SetHitObjects(Vector3 dir)
    {
        int dx = (int)dir.x, dy = (int)dir.y;
        hitFilter = hitOppositeFilter = hitPalette = hitFwall= null;
        hit = MapManager.Instance.gameGrid[objPos.x + dx * 2, objPos.y + dy * 2];
        List<GameObject> filters = MapManager.Instance.gameGrid[objPos.x + dx, objPos.y + dy];
        foreach (GameObject fo in filters)
        {
            if (!fo.activeSelf) continue;
            if (fo.CompareTag("Wall"))
            {
                hitFwall = fo;
                break;
            }
            IObject io = fo.GetComponent<IObject>();
            if (io.Type == ObjType.Palette) hitPalette = fo;
            else if (io.Type == ObjType.Filter)
            {
                Filter fiter = fo.GetComponent<Filter>();
                if (fiter.dir[(int)fiter.directionType] == dir) hitFilter = fo;
                else hitOppositeFilter = fo;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall") return;
        IObject io = collision.gameObject.GetComponent<IObject>();

        if (Type == ObjType.Player && io.Type == ObjType.Tile)
            return;
        if (io.Type == ObjType.Filter)
            return;
        if (isAlpha != io.isAlpha&&  !(Type==ObjType.Eraser|| Type == ObjType.BlackSmoke))
            return;
        if(interObj==null||isMoving)
        {     
            if(io!=null)
                interObj = io;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interObj == null)
            return;
        if (interObj.gameObject == collision.gameObject)
            interObj = null;
    }
    public void RefreshInteractObject()
    {
        interObj = null;
    }
    public void CompleteInteract(IObject other)
    {
        RefreshInteractObject();
        other.RefreshInteractObject();
        other.gameObject.SetActive(false);
    }
    public virtual void OnAlpha()
    {
        isAlpha = true;
        if(colorType!=ColorType.None) spriter.color = new Color(spriter.color.r, spriter.color.g, spriter.color.b, 0.5f);
        SpriteRenderer sre=GetComponent<SpriteRenderer>();
        if(spriter!=sre)
        {
            sre.color= new Color(sre.color.r, sre.color.g, sre.color.b, 0.5f);
        }    
    }

    public virtual void OffAlpha()
    {
        isAlpha = false;
        if (colorType != ColorType.None)
            spriter.color = new Color(spriter.color.r, spriter.color.g, spriter.color.b, 1f);
        SpriteRenderer sre = GetComponent<SpriteRenderer>();
        if (spriter != sre)
        {
            sre.color = new Color(sre.color.r, sre.color.g, sre.color.b, 1f);
        }
    }
}

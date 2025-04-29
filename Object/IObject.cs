using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using MyBox;
using TMPro;

public class ObjData
{
    public Vector3 pos;
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
    
    public ObjData(Vector3 pos, Vector3 faceDir,bool isActive, bool isAcryl, bool isAlpha, bool isBrush, bool isSuperAcryl,bool isTrail,uint sandCount, ColorType colorType,ColorType acrylColor)
    {
        this.pos = pos;
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
    Filter,
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

    private List<ObjType> NoneColorObjList =
        new List<ObjType>() { ObjType.Mop, ObjType.TrailBrush, ObjType.Water_Bucket, ObjType.WoodHammer };
    protected Collider2D[] hit;
    protected Collider2D filterObj, oppositeFilterObj;
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
        hit = Physics2D.OverlapBoxAll(transform.position+dir, Vector2.zero, 0);
        filterObj = Physics2D.OverlapBox(transform.position + dir*0.51f, Vector2.zero, 0);
        oppositeFilterObj = Physics2D.OverlapBox(transform.position + dir * 0.49f, Vector2.zero, 0);
        if (filterObj)
        {
            if (filterObj.CompareTag("Wall")) return false;
            
            Filter filter = filterObj.GetComponent<Filter>();
            if (filter && ((filter.isOneTime && filter.used) || (filter.filterType==FilterType.None && filter.colorType != colorType)))
            {
                return false;
            }

            Palette palette = filterObj.GetComponent<Palette>();
            if (palette) return false;
        }
        else if (oppositeFilterObj)
        {
            return false;
        }

        foreach (Collider2D h in hit)
        {
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (h.CompareTag("Wall"))
                return false;
            if ( obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl))
            {
                if (obj.MoveCheck(dir) == false)
                    return false;
            }
        }

        return true;
    }

    public virtual void Move(Vector3 dir)
    {
        if (filterObj)
        {
            Filter filter = filterObj.GetComponent<Filter>();
            if (filter.isOneTime)
            {
                filter.Use();
            }

            if (filter.filterType == FilterType.Mix)
            {
                colorType = PCHManager.MixColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
            else if (filter.filterType == FilterType.Substract)
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
            foreach (Collider2D h in hit)
            {
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
        
        foreach (Collider2D h in hit)
        {
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (h.CompareTag("Wall")) continue;
            if ( obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl))
            {
                obj.Move(dir);
            }
        }
        
        isMoving = true;
        StartCoroutine(MoveCoroutine(dir));
    }
    
    protected IEnumerator MoveCoroutine(Vector3 dir)
    {
        Vector3 dest = transform.position + dir * GameManager.Instance.unitDistance;
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
        if (Type != ObjType.TrailBrush && Type != ObjType.Water_Bucket  && Type!=ObjType.Fixed_Water_Bucket && Type !=ObjType.WoodHammer && Type!=ObjType.Mop)
        {
            colorType = cT;
            spriter.color = cT.ToColor();
        }
    }

    public virtual void SaveData()
    {
        ObjData newData = new ObjData(transform.position, Vector3.left, gameObject.activeSelf, isAcryl, isAlpha, isBrush, isSuperAcryl,isTrail,sandCount,colorType,acrylColor);
        moveLog.Push(newData);
    }

    public virtual void Undo()
    {
        if (moveLog.Count == 0)
        {
            MapManager.Instance.DeleteColorTile(gameObject.GetComponent<IObject>());
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
        StateUpdate();
        ColorChange(colorType);
        if (transform.position != lastData.pos)
            StartCoroutine(MoveCoroutine((lastData.pos - transform.position).normalized));
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall") return;
        IObject io = collision.gameObject.GetComponent<IObject>();

        if (Type == ObjType.Player && io.Type == ObjType.Tile)
            return;
        if (io.Type == ObjType.Filter)
            return;
        if (isAlpha != io.isAlpha)
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
    public void OnAlpha()
    {
        isAlpha = true;
        spriter.color = new Color(spriter.color.r, spriter.color.g, spriter.color.b, 0.5f);
    }
    public void OffAlpha()
    {
        isAlpha = false;
        spriter.color = new Color(spriter.color.r, spriter.color.g, spriter.color.b, 1f);
    }
}
